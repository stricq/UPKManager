using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

using Newtonsoft.Json;

using UpkManager.Domain.Contracts;
using UpkManager.Domain.Helpers;
using UpkManager.Domain.Models;
using UpkManager.Domain.Models.UpkFile;
using UpkManager.Domain.Models.UpkFile.Tables;

using UpkManager.Repository.Extensions;

using static System.IO.Path;


namespace UpkManager.Repository.Services {

  [Export(typeof(IUpkFileRepository))]
  public sealed class UpkFileRepository : IUpkFileRepository {

    #region Private Fields

    private bool hashUpdated;

    private ConcurrentDictionary<string, Tuple<DateTime, string>> buildHashes;

    private static Dictionary<string, Tuple<DateTime, string>> fileHashes;

    #endregion Private Fields

    #region IUpkFileRepository Implementation

    public async Task LoadDirectoryRecursiveFlat(List<DomainUpkFile> ParentFiles, string ParentPath, string Path, string Filter, bool isRoot = true) {
      if (isRoot) {
        await loadFileHashes();

        buildHashes = new ConcurrentDictionary<string, Tuple<DateTime, string>>();

        hashUpdated = false;
      }

      DirectoryInfo   dirInfo  = new DirectoryInfo(Path);
      DirectoryInfo[] dirInfos = await Task.Run(() => dirInfo.GetDirectories());

      if (dirInfos.Length > 0) {
        List<DomainUpkFile> dirs = dirInfos.Select(dir => new DomainUpkFile { GameFilename = dir.FullName.Replace(ParentPath, null) }).ToList();

        foreach(DomainUpkFile upkFile in dirs.ToList()) {
          List<DomainUpkFile> children = new List<DomainUpkFile>();

          await LoadDirectoryRecursiveFlat(children, ParentPath, Combine(ParentPath, upkFile.GameFilename), Filter, false);

          if (children.Count == 0) dirs.Remove(upkFile);
          else ParentFiles.AddRange(children);
        }
      }

      FileInfo[] files = await Task.Run(() => dirInfo.GetFiles(Filter));

      if (files.Length > 0) {
        ConcurrentBag<DomainUpkFile> upkFiles = new ConcurrentBag<DomainUpkFile>();

        await Task.Run(() => Parallel.ForEach(files, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, f => {
          string package = GetFileNameWithoutExtension(f.Name).ToLowerInvariant();

          string fileHash = null;

          if (f.DirectoryName?.EndsWith("cookedpc", StringComparison.CurrentCultureIgnoreCase) ?? false) {
            Tuple<DateTime, string> tuple = fileHashes.ContainsKey(package) ? fileHashes[package] : null;

            if (tuple != null && tuple.Item1 == f.LastWriteTime) {
              fileHash = tuple.Item2;

              buildHashes.TryAdd(package, tuple);
            }
            else {
              fileHash = f.OpenRead().GetHash<MD5>((int)f.Length);

              buildHashes.TryAdd(package, Tuple.Create(f.LastWriteTime, fileHash));

              hashUpdated = true;
            }
          }

          upkFiles.Add(new DomainUpkFile {
            GameFilename = f.FullName.Replace(ParentPath, null),
            ContentsRoot = f.FullName.Replace(ParentPath, null).Split('\\')[0].ToLowerInvariant(),
            Package      = package,
            Filesize     = f.Length,
            Filehash     = fileHash
          });
        }));

        ParentFiles.AddRange(upkFiles);
      }

      if (isRoot && hashUpdated) {
        await saveFileHashes(buildHashes.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));

        buildHashes = null;
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

    public async Task<DomainVersion> GetGameVersion(string GamePath) {
      const string matchLine = "ProductVersion";

      string versionFile = Combine(GamePath, @"..\bin\Version.ini");

      string version = await getConfigLine(versionFile, matchLine);

      return new DomainVersion(version);
    }

    public async Task<string> GetGameLocale(string GamePath) {
      const string matchLine = "Language";

      string localFile = Combine(GamePath, @"..\bin\local.ini");

      string locale = await getConfigLine(localFile, matchLine);

      return locale;
    }

    #endregion IUpkFileRepository Implementation

    #region Private Methods

    private static string HashFilename => Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"STR Programming Services\UPK Manager\FileHashes.json");

    private static async Task saveFileHashes(Dictionary<string, Tuple<DateTime, string>> hashes) {
      string json = await Task.Run(() => JsonConvert.SerializeObject(hashes, Formatting.Indented));

      if (!await Task.Run(() => File.Exists(HashFilename))) await Task.Run(() => Directory.CreateDirectory(GetDirectoryName(HashFilename)));

      await Task.Run(() => File.WriteAllText(HashFilename, json));
    }

    private static async Task loadFileHashes() {
      Dictionary<string, Tuple<DateTime, string>> hashes = null;

      if (await Task.Run(() => File.Exists(HashFilename))) {
        hashes = await Task.Run(() => JsonConvert.DeserializeObject<Dictionary<string, Tuple<DateTime, string>>>(File.ReadAllText(HashFilename)));
      }

      fileHashes = hashes ?? new Dictionary<string, Tuple<DateTime, string>>();
    }

    private static async Task<string> getConfigLine(string configFile, string configItem) {
      if (!File.Exists(configFile)) throw new FileNotFoundException($"Could not find the file: {configFile}");

      StreamReader stream = new StreamReader(File.OpenRead(configFile));

      string value = String.Empty;

      string line;

      while((line = await stream.ReadLineAsync()) != null) {
        if (line.StartsWith(configItem, StringComparison.CurrentCultureIgnoreCase)) {
          string[] parts = line.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);

          if (parts.Length > 1) value = parts[1];

          break;
        }
      }

      return value;
    }

    #endregion Private Methods

  }

}
