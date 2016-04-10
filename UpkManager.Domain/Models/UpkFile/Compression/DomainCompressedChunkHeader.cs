using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Helpers;


namespace UpkManager.Domain.Models.UpkFile.Compression {

  public class DomainCompressedChunkHeader {

    #region Constructor

    public DomainCompressedChunkHeader() {
      Blocks = new List<DomainCompressedChunkBlock>();
    }

    #endregion Constructor

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

        Blocks.Clear();

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

    public async Task<int> BuildCompressedChunkHeader(ByteArrayReader reader, uint flags) {
      Signature = Signatures.Signature;
      BlockSize = 0x00020000;

      CompressedSize   = 0;
      UncompressedSize = reader.Remaining;

      int blockCount = (reader.Remaining + BlockSize - 1) / BlockSize;

      int builderSize = 0;

      Blocks.Clear();

      for(int i = 0; i < blockCount; ++i) {
        DomainCompressedChunkBlock block = new DomainCompressedChunkBlock();

        ByteArrayReader uncompressed = await reader.ReadByteArray(Math.Min(BlockSize, reader.Remaining));

        builderSize += await block.BuildCompressedChunkBlockData(uncompressed);

        CompressedSize += block.CompressedSize;

        Blocks.Add(block);
      }

      builderSize += sizeof(uint)
                  +  sizeof(int) * 3;

      return builderSize;
    }

    public async Task<int> BuildExistingCompressedChunkHeader(ByteArrayReader reader, uint flags) {
      Signature = Signatures.Signature;
      BlockSize = 0x00020000;

      CompressedSize   = 0;
      UncompressedSize = reader.Remaining;

      int blockCount = (reader.Remaining + BlockSize - 1) / BlockSize;

      int builderSize = 0;

      for(int i = 0; i < blockCount; ++i) {
        builderSize += await Blocks[i].BuildExistingCompressedChunkBlockData();

        CompressedSize += Blocks[i].CompressedSize;
      }

      builderSize += sizeof(uint)
                  +  sizeof(int) * 3;

      return builderSize;
    }

    public async Task WriteCompressedChunkHeader(ByteArrayWriter Writer, int CurrentOffset) {
      Writer.WriteUInt32(Signature);

      Writer.WriteInt32(BlockSize);

      Writer.WriteInt32(CompressedSize);
      Writer.WriteInt32(UncompressedSize);

      foreach(DomainCompressedChunkBlock block in Blocks) await block.WriteCompressedChunkBlock(Writer);

      foreach(DomainCompressedChunkBlock block in Blocks) await block.WriteCompressedChunkBlockData(Writer);
    }

    #endregion Domain Methods

  }

}
