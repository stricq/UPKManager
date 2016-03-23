using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using UpkManager.Domain.Contracts;
using UpkManager.Domain.Helpers;
using UpkManager.Domain.Models;


namespace UpkManager.Repository.Services {

  [Export(typeof(IUpkFileRepository))]
  public class UpkFileRepository : IUpkFileRepository {

    #region IUpkFileRepository Implementation

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

      int headerSize = Header.GetBuilderSize();

      ByteArrayWriter writer = ByteArrayWriter.CreateNew(headerSize);

      await Header.WriteBuffer(writer, 0);

      await Task.Run(() => File.WriteAllBytes(Filename, writer.GetBytes()));
    }

    public async Task<int> GetGameVersion(string GamePath) {
      DirectoryInfo dirInfo = new DirectoryInfo(Path.Combine(GamePath, @"..\"));

      FileInfo[] files = dirInfo.GetFiles("VersionInfo_BNS*.ini");

      if (files.Length == 0) throw new FileNotFoundException("Could not find a matching VersionInfo_BNS*.ini file.");

      StreamReader stream = new StreamReader(File.OpenRead(files.First().FullName));

      int version = 0;

      string line;

      while((line = await stream.ReadLineAsync()) != null) {
        if (line.StartsWith("GlobalVersion", StringComparison.CurrentCultureIgnoreCase)) {
          string[] parts = line.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);

          if (parts.Length > 1) Int32.TryParse(parts[1], out version);

          break;
        }
      }

      return version;
    }

    #endregion IUpkFileRepository Implementation

  }

}
