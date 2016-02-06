using System;
using System.Collections.Generic;


namespace UpkManager.Entities.Compression {

  public class CompressedChunkHeader {

    #region Properties

    public uint Signature { get; set; }

    public int BlockSize { get; set; }

    public int CompressedSize { get; set; }

    public int UncompressedSize { get; set; }

    public List<CompressedChunkBlock> Blocks { get; set; }

    #endregion Properties

    #region Public Methods

    public void ReadCompressedChunkHeader(byte[] data, ref int index) {
      Signature = BitConverter.ToUInt32(data, index); index += sizeof(uint);

      BlockSize = BitConverter.ToInt32(data, index); index += sizeof(int);

      CompressedSize   = BitConverter.ToInt32(data, index); index += sizeof(int);
      UncompressedSize = BitConverter.ToInt32(data, index); index += sizeof(int);

      Blocks = new List<CompressedChunkBlock>();

      int blockCount = (UncompressedSize + BlockSize - 1) / BlockSize;

      for(int i = 0; i < blockCount; ++i) {
        CompressedChunkBlock block = new CompressedChunkBlock();

        block.ReadCompressedChunkBlock(data, ref index);

        Blocks.Add(block);
      }

      foreach(CompressedChunkBlock block in Blocks) block.ReadCompressedChunkBlockData(data, ref index);
    }

    #endregion Public Methods

  }

}
