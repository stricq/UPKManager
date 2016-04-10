using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Helpers;
using UpkManager.Domain.Models.UpkFile.Compression;
using UpkManager.Domain.Models.UpkFile.Tables;


namespace UpkManager.Domain.Models.UpkFile.Objects {

  public class DomainObjectCompressionBase : DomainObjectBase {

    #region Constructor

    public DomainObjectCompressionBase() {
      CompressedChunks = new List<DomainCompressedChunkBulkData>();
    }

    #endregion Constructor

    #region Properties

    protected byte[] Unknown1 { get; private set; }

    protected int CompressedChunkOffset { get; private set; }

    #endregion Properties

    #region Domain Properties

    protected List<DomainCompressedChunkBulkData> CompressedChunks { get; }

    #endregion Domain Properties

    #region Domain Methods

    public override async Task ReadDomainObject(ByteArrayReader reader, DomainHeader header, DomainExportTableEntry export, bool skipProperties, bool skipParse) {
      await base.ReadDomainObject(reader, header, export, skipProperties, skipParse);

      if (skipParse) return;

      Unknown1 = await reader.ReadBytes(sizeof(uint) * 3);

      CompressedChunkOffset = reader.ReadInt32();
    }

    protected async Task ProcessCompressedBulkData(ByteArrayReader reader, Func<DomainCompressedChunkBulkData, Task> chunkHandler) {
      DomainCompressedChunkBulkData compressedChunk = new DomainCompressedChunkBulkData();

//    CompressedChunks.Add(compressedChunk);

      await compressedChunk.ReadCompressedChunk(reader);

      await chunkHandler(compressedChunk);
    }

    protected async Task<int> ProcessUncompressedBulkData(ByteArrayReader reader, BulkDataCompressionTypes compressionFlags) {
      DomainCompressedChunkBulkData compressedChunk = new DomainCompressedChunkBulkData();

      CompressedChunks.Add(compressedChunk);

      int builderSize = await compressedChunk.BuildCompressedChunk(reader, compressionFlags);

      return builderSize;
    }

    protected async Task<int> ProcessExistingBulkData(int index, ByteArrayReader reader, BulkDataCompressionTypes compressionFlags) {
      int builderSize = await CompressedChunks[index].BuildExistingCompressedChunk(reader, compressionFlags);

      return builderSize;
    }

    #endregion Domain Methods

    #region DomainUpkBuilderBase Implementation

    public override int GetBuilderSize() {
      return sizeof(uint) * 3
           + sizeof(int);
    }

    public override async Task WriteBuffer(ByteArrayWriter Writer, int CurrentOffset) {
      await Writer.WriteBytes(Unknown1);

      Writer.WriteInt32(CurrentOffset + Writer.Index + sizeof(int));
    }

    #endregion DomainUpkBuilderBase Implementation

  }

}
