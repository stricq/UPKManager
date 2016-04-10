using System;
using System.Linq;
using System.Threading.Tasks;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Helpers;


namespace UpkManager.Domain.Models.UpkFile.Compression {

  public sealed class DomainCompressedChunkBulkData : DomainCompressedChunk {

    #region Private Fields

    private const BulkDataCompressionTypes NothingToDo = BulkDataCompressionTypes.Unused | BulkDataCompressionTypes.StoreInSeparatefile;

    #endregion Private Fields

    #region Properties

    public uint BulkDataFlags { get; private set; }

    #endregion Properties

    #region Domain Methods

    public override async Task ReadCompressedChunk(ByteArrayReader reader) {
      BulkDataFlags = reader.ReadUInt32();

      UncompressedSize = reader.ReadInt32();

      CompressedSize   = reader.ReadInt32();
      CompressedOffset = reader.ReadInt32();

      if (((BulkDataCompressionTypes)BulkDataFlags & NothingToDo) > 0) return;

      Header = new DomainCompressedChunkHeader();

      await Header.ReadCompressedChunkHeader(reader, BulkDataFlags, UncompressedSize, CompressedSize);
    }

    public async Task<ByteArrayReader> DecompressChunk(uint flags) {
      const BulkDataCompressionTypes nothingTodo = BulkDataCompressionTypes.Unused | BulkDataCompressionTypes.StoreInSeparatefile;

      if (((BulkDataCompressionTypes)BulkDataFlags & nothingTodo) > 0) return null;

      byte[] chunkData = new byte[Header.Blocks.Sum(block => block.UncompressedSize)];

      int uncompressedOffset = 0;

      foreach(DomainCompressedChunkBlock block in Header.Blocks) {
        if (((BulkDataCompressionTypes)BulkDataFlags & BulkDataCompressionTypes.LZO_ENC) > 0) await block.CompressedData.Decrypt();

        byte[] decompressed;

        const BulkDataCompressionTypes validCompression = BulkDataCompressionTypes.LZO | BulkDataCompressionTypes.LZO_ENC;

        if (((BulkDataCompressionTypes)BulkDataFlags & validCompression) > 0) decompressed = await block.CompressedData.Decompress(block.UncompressedSize);
        else {
          if (BulkDataFlags == 0) decompressed = block.CompressedData.GetBytes();
          else throw new Exception($"Unsupported bulk data compression type 0x{BulkDataFlags:X8}");
        }

        int offset = uncompressedOffset;

        await Task.Run(() => Array.ConstrainedCopy(decompressed, 0, chunkData, offset, block.UncompressedSize));

        uncompressedOffset += block.UncompressedSize;
      }

      return ByteArrayReader.CreateNew(chunkData, 0);
    }

    public async Task<int> BuildCompressedChunk(ByteArrayReader reader, BulkDataCompressionTypes compressionFlags) {
      BulkDataFlags = (uint)compressionFlags;

      int builderSize = sizeof(uint)
                      + sizeof(int) * 3;

      if ((compressionFlags & NothingToDo) > 0) return builderSize;

      reader.Seek(0);

      UncompressedSize = reader.Remaining;

      Header = new DomainCompressedChunkHeader();

      builderSize += await Header.BuildCompressedChunkHeader(reader, BulkDataFlags);

      CompressedSize = builderSize - 16;

      return builderSize;
    }

    public async Task<int> BuildExistingCompressedChunk(ByteArrayReader reader, BulkDataCompressionTypes compressionFlags) {
      BulkDataFlags = (uint)compressionFlags;

      int builderSize = sizeof(uint)
                      + sizeof(int) * 3;

      if ((compressionFlags & NothingToDo) > 0) return builderSize;

      reader.Seek(0);

      UncompressedSize = reader.Remaining;

      builderSize += await Header.BuildExistingCompressedChunkHeader(reader, BulkDataFlags);

      CompressedSize = builderSize - 16;

      return builderSize;
    }

    public async Task WriteCompressedChunk(ByteArrayWriter Writer, int CurrentOffset) {
      Writer.WriteUInt32(BulkDataFlags);

      if (((BulkDataCompressionTypes)BulkDataFlags & NothingToDo) > 0) {
        Writer.WriteInt32(0);
        Writer.WriteInt32(-1);

        Writer.WriteInt32(-1);

        return;
      }

      Writer.WriteInt32(UncompressedSize);
      Writer.WriteInt32(CompressedSize);

      Writer.WriteInt32(CurrentOffset + Writer.Index + sizeof(int));

      if (((BulkDataCompressionTypes)BulkDataFlags & BulkDataCompressionTypes.Unused) > 0) return;

      if (((BulkDataCompressionTypes)BulkDataFlags & BulkDataCompressionTypes.StoreInSeparatefile) > 0) return;

      await Header.WriteCompressedChunkHeader(Writer, CurrentOffset);
    }

    #endregion Domain Methods

  }

}
