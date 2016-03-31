using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Helpers;
using UpkManager.Domain.Models.Compression;
using UpkManager.Domain.Models.Tables;


namespace UpkManager.Domain.Models.Objects {

  public class DomainObjectCompressionBase : DomainObjectBase {

    #region Constructor

    public DomainObjectCompressionBase() {
      CompressedChunks = new List<DomainCompressedChunkBulkData>();
    }

    #endregion Constructor

    #region Properties

    public byte[] Unknown1 { get; private set; }

    public int CompressedChunkOffset { get; private set; }

    #endregion Properties

    #region Domain Properties

    public List<DomainCompressedChunkBulkData> CompressedChunks { get; }

    #endregion Domain Properties

    #region Domain Methods

    public override async Task ReadDomainObject(ByteArrayReader reader, DomainHeader header, DomainExportTableEntry export, bool skipProperties, bool skipParse) {
      await base.ReadDomainObject(reader, header, export, skipProperties, skipParse);

      if (skipParse) return;

      Unknown1 = await reader.ReadBytes(sizeof(uint) * 3);

      CompressedChunkOffset = reader.ReadInt32();
    }

    public async Task ProcessCompressedBulkData(ByteArrayReader reader, Func<DomainCompressedChunkBulkData, Task> chunkHandler) {
      DomainCompressedChunkBulkData compressedChunk = new DomainCompressedChunkBulkData();

      await compressedChunk.ReadCompressedChunk(reader);

      await chunkHandler(compressedChunk);
    }

    public async Task<int> ProcessUncompressedBulkData(ByteArrayReader reader, BulkDataCompressionTypes compressionFlags) {
      DomainCompressedChunkBulkData compressedChunk = new DomainCompressedChunkBulkData();

      CompressedChunks.Add(compressedChunk);

      int builderSize = await compressedChunk.BuildCompressedChunk(reader, compressionFlags);

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
