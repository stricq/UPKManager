using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Contracts;
using UpkManager.Domain.Models.Compression;
using UpkManager.Domain.Models.Tables;


namespace UpkManager.Domain.Models {

  public class DomainHeader {

    #region Private Fields

    private readonly IByteArrayReader reader;

    #endregion Private Fields

    #region Constructor

    public DomainHeader(IByteArrayReader Reader) {
      reader = Reader;

      Group = new DomainString();

      Generations = new List<DomainGenerationTableEntry>();

      CompressedChunks = new List<DomainCompressedChunk>();

      NameTable = new List<DomainNameTableEntry>();

      ExportTable = new List<DomainExportTableEntry>();
      ImportTable = new List<DomainImportTableEntry>();
    }

    #endregion Constructor

    #region Properties

    public uint Signature { get; set; }

    public ushort Version  { get; set; }

    public ushort Licensee { get; set; }

    public int Size { get; set; }

    public DomainString Group { get; set; }

    public uint Flags { get; set; }

    public int NameTableCount { get; set; }

    public int NameTableOffset { get; set; }

    public int ExportTableCount { get; set; }

    public int ExportTableOffset { get; set; }

    public int ImportTableCount { get; set; }

    public int ImportTableOffset { get; set; }

    public int DependsTableOffset { get; set; }

    public byte[] Guid { get; set; }

    public List<DomainGenerationTableEntry> Generations { get; set; }

    public uint EngineVersion { get; set; }

    public uint CookerVersion { get; set; }

    public uint CompressionFlags { get; set; }

    public List<DomainCompressedChunk> CompressedChunks { get; set; }

    public uint Unknown1 { get; set; }

    public uint Unknown2 { get; set; }

    public List<DomainNameTableEntry> NameTable { get; set; }

    public List<DomainExportTableEntry> ExportTable { get; set; }

    public List<DomainImportTableEntry> ImportTable { get; set; }

    public byte[] DependsTable { get; set; } // (Size - DependsOffset) bytes of data

    #endregion Properties

    #region Domain Properties

    public bool IsErrored { get; set; }

    public string FullFilename { get; set; }

    public string Filename => Path.GetFileName(FullFilename);

    public long FileSize { get; set; }

    #endregion Domain Properties

    #region Domain Methods

    public async Task ParseAsync() {
      await readUpkHeader();
    }

    public DomainObjectTableEntry GetObjectTableEntry(int reference) {
      if (reference < 0 && -reference - 1 < ImportTableCount) return ImportTable[-reference - 1];
      if (reference > 0 &&  reference - 1 < ExportTableCount) return ExportTable[reference - 1];

      if (reference == 0) return null;

      throw new Exception($"Object reference ({reference:X8}) is out of range of both the Import and Export Tables.");
    }

    #endregion Domain Methods

    #region Private Methods

    private async Task readUpkHeader() {
      reader.Seek(0);

      Signature = reader.ReadUInt32();

      if (Signature == FileHeader.EncryptedSignature) await reader.DecryptByteArray();
      else if (Signature != FileHeader.Signature) throw new Exception("File is not a properly formatted UPK file.");

      Version  = reader.ReadUInt16();
      Licensee = reader.ReadUInt16();

      Size = reader.ReadInt32();

      await Group.ReadString(reader);

      Flags = reader.ReadUInt32();

      NameTableCount  = reader.ReadInt32();
      NameTableOffset = reader.ReadInt32();

      ExportTableCount  = reader.ReadInt32();
      ExportTableOffset = reader.ReadInt32();

      ImportTableCount  = reader.ReadInt32();
      ImportTableOffset = reader.ReadInt32();

      DependsTableOffset = reader.ReadInt32();

      Guid = await reader.ReadBytes(16);

      Generations = await readGenerationsTable();

      EngineVersion = reader.ReadUInt32();
      CookerVersion = reader.ReadUInt32();

      CompressionFlags = reader.ReadUInt32();

      CompressedChunks = await readCompressedChunksTable();

      Unknown1 = reader.ReadUInt32();
      Unknown2 = reader.ReadUInt32();
    }

    private async Task<List<DomainGenerationTableEntry>> readGenerationsTable() {
      int count = reader.ReadInt32();

      List<DomainGenerationTableEntry> generations = new List<DomainGenerationTableEntry>();

      for(int i = 0; i < count; ++i) {
        DomainGenerationTableEntry info = new DomainGenerationTableEntry();

        await Task.Run(() => info.ReadGenerationTableEntry(reader));

        generations.Add(info);
      }

      return generations;
    }

    private async Task<List<DomainCompressedChunk>> readCompressedChunksTable() {
      int count = reader.ReadInt32();

      List<DomainCompressedChunk> chunks = new List<DomainCompressedChunk>();

      for(int i = 0; i < count; ++i) {
        DomainCompressedChunk chunk = new DomainCompressedChunk();

        await chunk.ReadCompressedChunk(reader);

        chunks.Add(chunk);
      }

      return chunks;
    }

    #endregion Private Methods

  }

}
