using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using UpkManager.Domain.Contracts;
using UpkManager.Domain.Helpers;
using UpkManager.Domain.Models;
using UpkManager.Domain.Models.UpkFile;
using UpkManager.Domain.Models.UpkFile.Tables;


namespace UpkManager.Repository.Services {

  [Export(typeof(IUpkFileRepository))]
  public sealed class UpkFileRepository : IUpkFileRepository {

    #region IUpkFileRepository Implementation

    public async Task LoadDirectoryRecursiveFlat(List<DomainUpkFile> ParentFiles, int Version, string ParentPath, string Path, string Filter) {
      DirectoryInfo   dirInfo  = new DirectoryInfo(Path);
      DirectoryInfo[] dirInfos = await Task.Run(() => dirInfo.GetDirectories());

      if (dirInfos.Length > 0) {
        List<DomainUpkFile> dirs = dirInfos.Select(dir => new DomainUpkFile { GameFilename = dir.FullName.Replace(ParentPath, null) }).ToList();

        foreach(DomainUpkFile upkFile in dirs.ToList()) {
          List<DomainUpkFile> children = new List<DomainUpkFile>();

          await LoadDirectoryRecursiveFlat(children, Version, ParentPath, System.IO.Path.Combine(ParentPath, upkFile.GameFilename), Filter);

          if (children.Count == 0) dirs.Remove(upkFile);
          else ParentFiles.AddRange(children);
        }
      }

      FileInfo[] files = await Task.Run(() => dirInfo.GetFiles(Filter));

      if (files.Length > 0) {
        List<DomainUpkFile> upkFiles = files.Select(f => new DomainUpkFile { GameFilename = f.FullName.Replace(ParentPath, null), FileSize = f.Length, GameVersion = Version }).ToList();

        ParentFiles.AddRange(upkFiles);
      }
    }

    public async Task LoadDirectoryRecursive(DomainExportedObject Parent, string Path) {
      DirectoryInfo   dirInfo  = new DirectoryInfo(Path);
      DirectoryInfo[] dirInfos = await Task.Run(() => dirInfo.GetDirectories());

      if (dirInfos.Length > 0) {
        Parent.Children = dirInfos.Select(dir => new DomainExportedObject { Filename = dir.FullName, Parent = Parent, Children = new List<DomainExportedObject>() }).ToList();

        foreach(DomainExportedObject exportedDir in Parent.Children) {
          await LoadDirectoryRecursive(exportedDir, exportedDir.Filename);
        }
      }

      FileInfo[] files = await Task.Run(() => dirInfo.GetFiles());

      if (files.Length > 0) {
        Parent.Children.AddRange(files.Select(f => new DomainExportedObject { Filename = f.FullName, Parent = Parent }).ToList());
      }
    }

    public async Task<DomainHeader> LoadUpkFile(string Filename) {
      byte[] data = await Task.Run(() => File.ReadAllBytes(Filename));

      ByteArrayReader reader = ByteArrayReader.CreateNew(data, 0);

      DomainHeader header = new DomainHeader(reader) {
        FullFilename = Filename,
        FileSize     = data.LongLength
      };

      return header;
    }

    public async Task SaveUpkFile(DomainHeader Header, string Filename) {
      if (Header == null) return;

      foreach(DomainExportTableEntry export in Header.ExportTable.Where(export => export.DomainObject == null)) await export.ParseDomainObject(Header, false, false);

      FileStream stream = new FileStream(Filename, FileMode.Create);

      int headerSize = Header.GetBuilderSize();

      ByteArrayWriter writer = ByteArrayWriter.CreateNew(headerSize);

      await Header.WriteBuffer(writer, 0);

      await stream.WriteAsync(writer.GetBytes(), 0, headerSize);

      foreach(DomainExportTableEntry export in Header.ExportTable) {
        ByteArrayWriter objectWriter = await export.WriteObjectBuffer();

        await stream.WriteAsync(objectWriter.GetBytes(), 0, objectWriter.Index);
      }

      await stream.FlushAsync();

      stream.Close();
    }

    public async Task<int> GetGameVersion(string GamePath) {
      DirectoryInfo dirInfo = new DirectoryInfo(Path.Combine(GamePath, @"..\"));

      FileInfo[] files = dirInfo.GetFiles("VersionInfo_BNS*.ini");

      string matchLine = "GlobalVersion";

      StreamReader stream;

      if (files.Length > 0) stream = new StreamReader(File.OpenRead(files.First().FullName));
      else {
        string gameVersion = Path.Combine(GamePath, @"..\bin\Version.ini");

        if (!File.Exists(gameVersion)) throw new FileNotFoundException("Could not find a matching VersionInfo_BNS*.ini file.");

        stream = new StreamReader(File.OpenRead(gameVersion));

        matchLine = "ProductVersion";
      }



      int version = 0;

      string line;

      while((line = await stream.ReadLineAsync()) != null) {
        if (line.StartsWith(matchLine, StringComparison.CurrentCultureIgnoreCase)) {
          string[] parts = line.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);

          if (parts.Length > 1) {
            string[] digits = parts[1].Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            for(int i = 0; i < digits.Length; ++i) {
              int segment;

              if (Int32.TryParse(digits[i], out segment)) version = version * 1000 + segment;
            }
          }

          break;
        }
      }

      return version;
    }

    #endregion IUpkFileRepository Implementation

  }

}
