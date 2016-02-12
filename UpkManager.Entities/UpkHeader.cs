using System;
using System.Collections.Generic;

using UpkManager.Entities.Compression;
using UpkManager.Entities.Constants;
using UpkManager.Entities.Tables;


namespace UpkManager.Entities {

  public class UpkHeader {

    #region Constructor

    public UpkHeader() {
      Group = new UpkString();

      Guid = new byte[16];

      NameTable = new List<NameTableEntry>();

      ExportTable = new List<ExportTableEntry>();
      ImportTable = new List<ImportTableEntry>();
    }

    #endregion Constructor

    #region Properties
    //
    // Parsed Properties
    //
    public uint Signature { get; set; }

    public ushort Version  { get; set; }

    public ushort Licensee { get; set; }

    public int Size { get; set; }

    public UpkString Group { get; set; }

    public uint Flags { get; set; }

    public int NameTableCount { get; set; }

    public int NameTableOffset { get; set; }

    public int ExportTableCount { get; set; }

    public int ExportTableOffset { get; set; }

    public int ImportTableCount { get; set; }

    public int ImportTableOffset { get; set; }

    public int DependsTableOffset { get; set; }

    public byte[] Guid { get; set; }

    public List<GenerationTableEntry> Generations { get; set; }

    public uint EngineVersion { get; set; }

    public uint CookerVersion { get; set; }

    public uint CompressionFlags { get; set; }

    public List<CompressedChunk> CompressedChunks { get; set; }

    public uint Unknown1 { get; set; }

    public uint Unknown2 { get; set; }

    public List<NameTableEntry> NameTable { get; set; }

    public List<ExportTableEntry> ExportTable { get; set; }

    public List<ImportTableEntry> ImportTable { get; set; }

    public byte[] DependsTable { get; set; } // (Size - DependsOffset) bytes of data

    #endregion Properties

    #region Public Methods

    public void ReadUpkHeader(byte[] data) {
      int index = 0;

      Signature = BitConverter.ToUInt32(data, index); index += sizeof(uint);

      if (Signature != FileHeader.Signature) throw new Exception("Missing signature.  File is not a properly formatted UPK file.");

      Version   = BitConverter.ToUInt16(data, index); index += sizeof(ushort);
      Licensee  = BitConverter.ToUInt16(data, index); index += sizeof(ushort);

      Size = BitConverter.ToInt32(data, index); index += sizeof(int);

      Group.ReadUpkStr(data, ref index);

      Flags = BitConverter.ToUInt32(data, index); index += sizeof(uint);

      NameTableCount  = BitConverter.ToInt32(data, index); index += sizeof(int);
      NameTableOffset = BitConverter.ToInt32(data, index); index += sizeof(int);

      ExportTableCount  = BitConverter.ToInt32(data, index); index += sizeof(int);
      ExportTableOffset = BitConverter.ToInt32(data, index); index += sizeof(int);

      ImportTableCount  = BitConverter.ToInt32(data, index); index += sizeof(int);
      ImportTableOffset = BitConverter.ToInt32(data, index); index += sizeof(int);

      DependsTableOffset = BitConverter.ToInt32(data, index); index += sizeof(int);

      Array.ConstrainedCopy(data, index, Guid, 0, 16); index += 16;

      Generations = readGenerationsTable(data, ref index);

      EngineVersion = BitConverter.ToUInt32(data, index); index += sizeof(uint);
      CookerVersion = BitConverter.ToUInt32(data, index); index += sizeof(uint);

      CompressionFlags = BitConverter.ToUInt32(data, index); index += sizeof(uint);

      CompressedChunks = readCompressedChunksTable(data, ref index);

      Unknown1 = BitConverter.ToUInt32(data, index); index += sizeof(uint);
      Unknown2 = BitConverter.ToUInt32(data, index);
    }

    public ObjectTableEntry GetObjectTableEntry(int reference) {
      if (reference < 0 && -reference - 1 < ImportTableCount) return ImportTable[-reference - 1];
      if (reference > 0 &&  reference - 1 < ExportTableCount) return ExportTable[reference - 1];

      if (reference == 0) return null;

      throw new Exception($"Object reference ({reference:X8}) is out of range of both the Import and Export Tables.");
    }

    #endregion Public Methods

    #region Private Methods

    private static List<GenerationTableEntry> readGenerationsTable(byte[] data, ref int index) {
      int count = BitConverter.ToInt32(data, index); index += sizeof(int);

      List<GenerationTableEntry> generations = new List<GenerationTableEntry>();

      for(int i = 0; i < count; ++i) {
        GenerationTableEntry info = new GenerationTableEntry();

        info.ReadGenerationTableEntry(data, ref index);

        generations.Add(info);
      }

      return generations;
    }

    private static List<CompressedChunk> readCompressedChunksTable(byte[] data, ref int index) {
      int count = BitConverter.ToInt32(data, index); index += sizeof(int);

      List<CompressedChunk> chunks = new List<CompressedChunk>();

      for(int i = 0; i < count; ++i) {
        CompressedChunk chunk = new CompressedChunk();

        chunk.ReadCompressedChunk(data, ref index);

        chunks.Add(chunk);
      }

      return chunks;
    }

    #endregion Private Methods

  }

}
