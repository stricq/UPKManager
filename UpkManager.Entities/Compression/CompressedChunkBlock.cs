using System;


namespace UpkManager.Entities.Compression {

  public class CompressedChunkBlock {

    #region Properties

    public int CompressedSize { get; set; }

    public int UncompressedSize { get; set; }

    public byte[] CompressedData { get; set; }

    #endregion Properties

    #region Public Methods

    public void ReadCompressedChunkBlock(byte[] data, ref int index) {
      CompressedSize   = BitConverter.ToInt32(data, index); index += sizeof(int);
      UncompressedSize = BitConverter.ToInt32(data, index); index += sizeof(int);
    }

    public void ReadCompressedChunkBlockData(byte[] data, ref int index) {
      CompressedData = new byte[CompressedSize];

      Array.ConstrainedCopy(data, index, CompressedData, 0, CompressedSize);

      index += CompressedSize;
    }

    #endregion Public Methods

  }

}
