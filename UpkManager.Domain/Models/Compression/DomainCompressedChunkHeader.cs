using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Helpers;


namespace UpkManager.Domain.Models.Compression {

  public class DomainCompressedChunkHeader {

    #region Properties

    public uint Signature { get; set; }

    public int BlockSize { get; set; }

    public int CompressedSize { get; set; }

    public int UncompressedSize { get; set; }

    public List<DomainCompressedChunkBlock> Blocks { get; set; }

    #endregion Properties

    #region Domain Methods

    public async Task ReadCompressedChunkHeader(ByteArrayReader reader, uint flags, int uncompressedSize, int compressedSize) {
      if (flags > 0) {
        Signature = reader.ReadUInt32();

        if (Signature != Signatures.Signature) throw new Exception("Compressed Header Signature not found.");

        BlockSize = reader.ReadInt32();

        CompressedSize   = reader.ReadInt32();
        UncompressedSize = reader.ReadInt32();

        Blocks = new List<DomainCompressedChunkBlock>();

        int blockCount = (UncompressedSize + BlockSize - 1) / BlockSize;

        for(int i = 0; i < blockCount; ++i) {
          DomainCompressedChunkBlock block = new DomainCompressedChunkBlock();

          block.ReadCompressedChunkBlock(reader);

          Blocks.Add(block);
        }
      }
      else {
        Blocks = new List<DomainCompressedChunkBlock> {
          new DomainCompressedChunkBlock {
            UncompressedSize = uncompressedSize,
              CompressedSize =   compressedSize
          }
        };
      }

      foreach(DomainCompressedChunkBlock block in Blocks) await block.ReadCompressedChunkBlockData(reader);
    }

    #endregion Domain Methods

  }

}
