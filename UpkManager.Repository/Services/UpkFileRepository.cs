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

    #endregion IUpkFileRepository Implementation

  }

}
