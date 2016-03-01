using System.Threading.Tasks;

using UpkManager.Domain.Contracts;


namespace UpkManager.Domain.Models.Compression {

  public class DomainCompressedChunkBlock {

    #region Properties

    public int CompressedSize { get; set; }

    public int UncompressedSize { get; set; }

    public byte[] CompressedData { get; set; }

    #endregion Properties

    #region Domain Methods

    public void ReadCompressedChunkBlock(IByteArrayReader reader) {
      CompressedSize   = reader.ReadInt32();
      UncompressedSize = reader.ReadInt32();
    }

    public async Task ReadCompressedChunkBlockData(IByteArrayReader reader) {
      CompressedData = await reader.ReadBytes(CompressedSize);
    }

    #endregion Domain Methods

  }

}
