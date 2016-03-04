using System.Threading.Tasks;

using UpkManager.Domain.Helpers;


namespace UpkManager.Domain.Models.Compression {

  public class DomainCompressedChunkBlock {

    #region Properties

    public int CompressedSize { get; set; }

    public int UncompressedSize { get; set; }

    public ByteArrayReader CompressedData { get; set; }

    #endregion Properties

    #region Domain Methods

    public void ReadCompressedChunkBlock(ByteArrayReader reader) {
      CompressedSize   = reader.ReadInt32();
      UncompressedSize = reader.ReadInt32();
    }

    public async Task ReadCompressedChunkBlockData(ByteArrayReader reader) {
      CompressedData = await reader.ReadByteArray(CompressedSize);
    }

    #endregion Domain Methods

  }

}
