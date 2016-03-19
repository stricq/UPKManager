using System;
using System.Threading.Tasks;

using UpkManager.Domain.Helpers;
using UpkManager.Domain.Models.Compression;
using UpkManager.Domain.Models.Tables;


namespace UpkManager.Domain.Models.Objects {

  public class DomainObjectCompressionBase : DomainObjectBase {

    #region Properties

    public byte[] Unknown1 { get; private set; }

    public int CompressedChunkOffset { get; private set; }

    #endregion Properties

    #region Domain Methods

    public override async Task ReadDomainObject(ByteArrayReader reader, DomainHeader header, DomainExportTableEntry export, bool skipProperties, bool skipParse) {
      await base.ReadDomainObject(reader, header, export, skipProperties, skipParse);

      if (skipParse) return;

      Unknown1 = await reader.ReadBytes(3 * sizeof(uint));

      CompressedChunkOffset = reader.ReadInt32();
    }

    public virtual async Task ProcessCompressedBulkData(ByteArrayReader reader, Func<DomainCompressedChunkBulkData, Task> chunkHandler) {
      DomainCompressedChunkBulkData compressedChunk = new DomainCompressedChunkBulkData();

      await compressedChunk.ReadCompressedChunk(reader);

      await chunkHandler(compressedChunk);
    }

    #endregion Domain Methods

  }

}
