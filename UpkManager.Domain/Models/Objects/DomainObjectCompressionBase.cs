using System;
using System.Threading.Tasks;

using UpkManager.Domain.Helpers;
using UpkManager.Domain.Models.Compression;
using UpkManager.Domain.Models.Tables;


namespace UpkManager.Domain.Models.Objects {

  public class DomainObjectCompressionBase : DomainObjectBase {

    #region Properties

    public byte[] Unknown1 { get; set; }

    public int CompressedChunkOffset { get; set; }

    public int CompressedChunkCount { get; set; }

    #endregion Properties

    #region Domain Methods

    public override async Task ReadDomainObject(ByteArrayReader reader, DomainHeader header, DomainExportTableEntry export, bool skipProperties, bool skipParse) {
      await base.ReadDomainObject(reader, header, export, skipProperties, skipParse);

      if (skipParse) return;

      Unknown1 = await reader.ReadBytes(3 * sizeof(uint));

      CompressedChunkOffset = reader.ReadInt32();
      CompressedChunkCount  = reader.ReadInt32();
    }

    public virtual async Task ProcessCompressedBulkData(ByteArrayReader reader, Func<DomainCompressedChunkBulkData, Task> chunkHandler) {
      for(int i = 0; i < CompressedChunkCount; ++i) {
        DomainCompressedChunkBulkData bulkChunk = new DomainCompressedChunkBulkData();

        await bulkChunk.ReadCompressedChunk(reader);

        await chunkHandler(bulkChunk);
      }
    }

    #endregion Domain Methods

  }

}
