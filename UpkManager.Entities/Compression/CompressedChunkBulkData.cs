using System;
using System.Linq;

using UpkManager.Entities.Constants;

using ManagedLZO;


namespace UpkManager.Entities.Compression {

  public class CompressedChunkBulkData : CompressedChunk {

    #region Properties

    public uint BulkDataFlags { get; set; }

    #endregion Properties

    #region Overrides

    public override void ReadCompressedChunk(byte[] data, ref int index) {
      BulkDataFlags = BitConverter.ToUInt32(data, index); index += sizeof(uint);

      UncompressedSize = BitConverter.ToInt32(data, index); index += sizeof(int);

      CompressedSize   = BitConverter.ToInt32(data, index); index += sizeof(int);
      CompressedOffset = BitConverter.ToInt32(data, index); index += sizeof(int);

      if ((BulkDataFlags & BulkDataFlag.Unused) == BulkDataFlag.Unused) return;

      Header = new CompressedChunkHeader();

      Header.ReadCompressedChunkHeader(data, ref index);
    }

    public override byte[] DecompressChunk(uint flags) {
      if ((BulkDataFlags & BulkDataFlag.Unused) == BulkDataFlag.Unused) return null;

      byte[] chunkData = new byte[Header.Blocks.Sum(block => block.UncompressedSize)];

      int uncompressedOffset = 0;

      foreach(CompressedChunkBlock block in Header.Blocks) {
        if ((BulkDataFlags & BulkDataFlag.CompressedLzoEnc) == BulkDataFlag.CompressedLzoEnc) DecryptChunk(block.CompressedData);

        byte[] decompressed = new byte[block.UncompressedSize];

        MiniLZO.Decompress(block.CompressedData, decompressed);

//      block.UncompressedOffset = chunk.UncompressedOffset + uncompressedOffset;

        int offset = uncompressedOffset;

        Array.ConstrainedCopy(decompressed, 0, chunkData, offset, block.UncompressedSize);

        uncompressedOffset += block.UncompressedSize;
      }

      return chunkData;
    }

    #endregion Overrides

  }

}
