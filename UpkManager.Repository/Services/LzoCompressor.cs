using System.ComponentModel.Composition;
using System.Threading.Tasks;

using ManagedLZO;

using UpkManager.Repository.Contracts;


namespace UpkManager.Repository.Services {

  [Export(typeof(ILzoCompressor))]
  public class LzoCompressor : ILzoCompressor {

    #region ILzoCompressor Implementation

    public async Task<byte[]> DecompressAsync(byte[] Source, int UncompressedSize) {
      int length = UncompressedSize;

      byte[] destination = new byte[length];

      await Task.Run(() => MiniLZO.Decompress(Source, destination));

      return destination;
    }

    #endregion ILzoCompressor Implementation

  }

}
