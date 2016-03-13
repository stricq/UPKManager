using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;

using UpkManager.Domain.Contracts;
using UpkManager.Domain.Helpers;
using UpkManager.Domain.Models;


namespace UpkManager.Repository.Services {

  [Export(typeof(IUpkFileRepository))]
  public class UpkFileRepository : IUpkFileRepository {

    #region IUpkFileRepository Implementation

    public async Task<DomainHeader> LoadUpkFile(string filename) {
      byte[] data = await Task.Run(() => File.ReadAllBytes(filename));

      ByteArrayReader reader = ByteArrayReader.CreateNew(data, 0);

      DomainHeader header = new DomainHeader(reader) {
        FullFilename = filename,
        FileSize     = data.LongLength
      };

      return header;
    }

    public async Task<int> GetGameVersion(string gamePath) {
      StreamReader stream = new StreamReader(File.OpenRead(Path.Combine(gamePath, @"..\VersionInfo_BnS.ini")));

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
