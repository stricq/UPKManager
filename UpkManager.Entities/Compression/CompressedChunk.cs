using System;


namespace UpkManager.Entities.Compression {

  public class CompressedChunk {

    #region Properties

    public int UncompressedOffset { get; set; }

    public int UncompressedSize { get; set; }

    public int CompressedOffset { get; set; }

    public int CompressedSize { get; set; }

    public CompressedChunkHeader Header { get; set; }

    #endregion Properties

    #region Public Methods

    public virtual void ReadCompressedChunk(byte[] data, ref int index) {
      UncompressedOffset = BitConverter.ToInt32(data, index); index += sizeof(int);
      UncompressedSize   = BitConverter.ToInt32(data, index); index += sizeof(int);

      CompressedOffset = BitConverter.ToInt32(data, index); index += sizeof(int);
      CompressedSize   = BitConverter.ToInt32(data, index); index += sizeof(int);

      Header = new CompressedChunkHeader();

      int cIndex = CompressedOffset;

      Header.ReadCompressedChunkHeader(data, ref cIndex);
    }

    public virtual byte[] DecompressChunk(uint flags) {
      return new byte[0];
    }

    #endregion Public Methods

    #region Protected Methods

    protected static void DecryptChunk(byte[] data) {
      if (data.Length < 32) return;

//    const string key = "qiffjdlerdoqymvketdcl0er2subioxq";

      byte[] key = { 0x71, 0x69, 0x66, 0x66, 0x6a, 0x64, 0x6c, 0x65, 0x72, 0x64, 0x6f, 0x71, 0x79, 0x6d, 0x76, 0x6b, 0x65, 0x74, 0x64, 0x63, 0x6c, 0x30, 0x65, 0x72, 0x32, 0x73, 0x75, 0x62, 0x69, 0x6f, 0x78, 0x71 };

      for(int i = 0; i < data.Length; ++i) data[i] ^= key[i % 32];
    }

    #endregion Protected Methods

  }

}
