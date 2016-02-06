using System.Threading.Tasks;


namespace UpkManager.Repository.Contracts {

  public interface ILzoCompressor {

    Task<byte[]> DecompressAsync(byte[] Source, int UncompressedSize);

  }

}
