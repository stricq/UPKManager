using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using STR.Common.Extensions;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Helpers;
using UpkManager.Domain.Models.UpkFile.Compression;
using UpkManager.Domain.Models.UpkFile.Tables;


namespace UpkManager.Domain.Models.UpkFile {

  public sealed class DomainHeader : DomainHeaderBuilderBase {

    #region Private Fields

    private ByteArrayReader reader;

    private ByteArrayWriter writer;

    #endregion Private Fields

    #region Constructor

    public DomainHeader(ByteArrayReader Reader) {
      reader = Reader;

      Group = new DomainString();

      GenerationTable = new List<DomainGenerationTableEntry>();

      CompressedChunks = new List<DomainCompressedChunk>();

      NameTable = new List<DomainNameTableEntry>();

      ExportTable = new List<DomainExportTableEntry>();
      ImportTable = new List<DomainImportTableEntry>();
    }

    #endregion Constructor

    #region Properties

    public uint Signature { get; private set; }

    public ushort Version  { get; private set; }

    public ushort Licensee { get; private set; }

    public int Size { get; private set; }

    public DomainString Group { get; }

    public uint Flags { get; private set; }

    public int NameTableCount { get; private set; }

    public int NameTableOffset { get; private set; }

    public int ExportTableCount { get; private set; }

    public int ExportTableOffset { get; private set; }

    public int ImportTableCount { get; private set; }

    public int ImportTableOffset { get; private set; }

    public int DependsTableOffset { get; private set; }

    public byte[] Guid { get; private set; }

    public int GenerationTableCount { get; private set; }

    public List<DomainGenerationTableEntry> GenerationTable { get; private set; }

    public uint EngineVersion { get; private set; }

    public uint CookerVersion { get; private set; }

    public uint CompressionFlags { get; private set; }

    public int CompressionTableCount { get; private set; }

    public List<DomainCompressedChunk> CompressedChunks { get; private set; }

    public uint Unknown1 { get; private set; }

    public uint Unknown2 { get; private set; }

    public List<DomainNameTableEntry> NameTable { get; }

    public List<DomainExportTableEntry> ExportTable { get; }

    public List<DomainImportTableEntry> ImportTable { get; }

    public byte[] DependsTable { get; private set; } // (Size - DependsOffset) bytes; or ExportTableCount * 4 bytes;

    #endregion Properties

    #region Domain Properties

    public string FullFilename { get; set; }

    public string Filename => Path.GetFileName(FullFilename);

    public long FileSize { get; set; }

    #endregion Domain Properties

    #region Domain Methods

    public async Task ReadHeaderAsync(Action<DomainLoadProgress> progress) {
      DomainLoadProgress message = new DomainLoadProgress { Text = "Parsing Header..." };

      progress?.Invoke(message);

      await readUpkHeader();

      const CompressionTypes validCompression = CompressionTypes.LZO | CompressionTypes.LZO_ENC;

      if (((CompressionTypes)CompressionFlags & validCompression) > 0 ) {
        message.Text = "Decompressing...";

        progress?.Invoke(message);

        reader = await decompressChunks();
      }
      else if (CompressionFlags > 0) throw new Exception($"Unsupported compression type 0x{CompressionFlags:X8}.");

      await readNameTable(progress);

      await readImportTable(progress);

      await readExportTable(progress);

      message.Text = "Slicing and Dicing...";

      progress?.Invoke(message);

      await readDependsTable();

      await decodePointers();

      message.Text  = "Reading Objects...";
      message.Total = ExportTableCount;

      progress?.Invoke(message);

      await ExportTable.ForEachAsync(export => {
        return export.ReadDomainObject(reader).ContinueWith(t => {
          message.IncrementCurrent();

          if (ExportTableCount > 100) progress?.Invoke(message);
        });
      });

      message.IsComplete = true;

      progress?.Invoke(message);
    }

    public DomainObjectTableEntryBase GetObjectTableEntry(int reference) {
      if (reference == 0) return null;

      if (reference < 0 && -reference - 1 < ImportTableCount) return ImportTable[-reference - 1];
      if (reference > 0 &&  reference - 1 < ExportTableCount) return ExportTable[reference - 1];

      throw new Exception($"Object reference ({reference:X8}) is out of range of both the Import and Export Tables.");
    }

    #endregion Domain Methods

    #region DomainUpkBuilderBase Implementation

    public override int GetBuilderSize() {
      if (CompressedChunks.Any()) throw new NotSupportedException("Cannot rebuild compressed files. Yet.");

      if (Signature == Signatures.EncryptedSignature) throw new NotSupportedException("Cannot rebuild fully encrypted files. Yet.");

      BuilderSize = sizeof(uint)   * 7
                  + sizeof(ushort) * 2
                  + sizeof(int)    * 10
                  + Group.GetBuilderSize()
                  + Guid.Length
                  + GenerationTable.Sum(gen => gen.GetBuilderSize());

      BuilderNameTableOffset = BuilderSize;

      BuilderSize += NameTable.Sum(name => name.GetBuilderSize());

      BuilderImportTableOffset = BuilderSize;

      BuilderSize += ImportTable.Sum(import => import.GetBuilderSize());

      BuilderExportTableOffset = BuilderSize;

      BuilderSize += ExportTable.Sum(export => export.GetBuilderSize());

      BuilderDependsTableOffset = BuilderSize;

      BuilderSize += DependsTable.Length;

      ExportTable.Aggregate(BuilderSize, (current, export) => current + export.GetObjectSize(current));

      return BuilderSize;
    }

    public override async Task WriteBuffer(ByteArrayWriter Writer, int CurrentOffset) {
      writer = Writer;

      await writeUpkHeader();

      await writeNameTable();

      await writeImportTable();

      await encodePointers();

      await writeExportTable();

      await writeDependsTable();
    }

    #endregion DomainUpkBuilderBase Implementation

    #region Private Methods

    private async Task readUpkHeader() {
      reader.Seek(0);

      Signature = reader.ReadUInt32();

      if (Signature == Signatures.EncryptedSignature) await reader.Decrypt();
      else if (Signature != Signatures.Signature) throw new Exception("File is not a properly formatted UPK file.");

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

      GenerationTableCount = reader.ReadInt32();

      GenerationTable = await readGenerationTable();

      EngineVersion = reader.ReadUInt32();
      CookerVersion = reader.ReadUInt32();

      CompressionFlags = reader.ReadUInt32();

      CompressionTableCount = reader.ReadInt32();

      CompressedChunks = await readCompressedChunksTable();

      Unknown1 = reader.ReadUInt32();
      Unknown2 = reader.ReadUInt32();
    }

    private async Task writeUpkHeader() {
      writer.Seek(0);

      writer.WriteUInt32(Signature);

      writer.WriteUInt16(Version);
      writer.WriteUInt16(Licensee);

      writer.WriteInt32(BuilderSize);

      await Group.WriteBuffer(writer, 0);

      writer.WriteUInt32(Flags);

      writer.WriteInt32(NameTable.Count);
      writer.WriteInt32(BuilderNameTableOffset);

      writer.WriteInt32(ExportTable.Count);
      writer.WriteInt32(BuilderExportTableOffset);

      writer.WriteInt32(ImportTable.Count);
      writer.WriteInt32(BuilderImportTableOffset);

      writer.WriteInt32(BuilderDependsTableOffset);

      await writer.WriteBytes(Guid);

      writer.WriteInt32(GenerationTable.Count);

      await writeGenerationTable();

      writer.WriteUInt32(EngineVersion);
      writer.WriteUInt32(CookerVersion);

      writer.WriteUInt32(CompressionFlags);

      writer.WriteInt32(CompressedChunks.Count);

      writer.WriteUInt32(Unknown1);
      writer.WriteUInt32(Unknown2);
    }

    private async Task<List<DomainGenerationTableEntry>> readGenerationTable() {
      List<DomainGenerationTableEntry> generations = new List<DomainGenerationTableEntry>();

      for(int i = 0; i < GenerationTableCount; ++i) {
        DomainGenerationTableEntry info = new DomainGenerationTableEntry();

        await Task.Run(() => info.ReadGenerationTableEntry(reader));

        generations.Add(info);
      }

      return generations;
    }

    private async Task writeGenerationTable() {
      foreach(DomainGenerationTableEntry entry in GenerationTable) {
        await entry.WriteBuffer(writer, 0);
      }
    }

    private async Task<List<DomainCompressedChunk>> readCompressedChunksTable() {
      List<DomainCompressedChunk> chunks = new List<DomainCompressedChunk>();

      for(int i = 0; i < CompressionTableCount; ++i) {
        DomainCompressedChunk chunk = new DomainCompressedChunk();

        await chunk.ReadCompressedChunk(reader);

        chunks.Add(chunk);
      }

      return chunks;
    }

    private async Task<ByteArrayReader> decompressChunks() {
      int start = CompressedChunks.Min(ch => ch.UncompressedOffset);

      int totalSize = CompressedChunks.SelectMany(ch => ch.Header.Blocks).Aggregate(start, (total, block) => total + block.UncompressedSize);

      byte[] data = new byte[totalSize];

      foreach(DomainCompressedChunk chunk in CompressedChunks) {
        byte[] chunkData = new byte[chunk.Header.Blocks.Sum(block => block.UncompressedSize)];

        int uncompressedOffset = 0;

        foreach(DomainCompressedChunkBlock block in chunk.Header.Blocks) {
          if (((CompressionTypes)CompressionFlags & CompressionTypes.LZO_ENC) > 0) await block.CompressedData.Decrypt();

          byte[] decompressed = await block.CompressedData.Decompress(block.UncompressedSize);

          int offset = uncompressedOffset;

          await Task.Run(() => Array.ConstrainedCopy(decompressed, 0, chunkData, offset, block.UncompressedSize));

          uncompressedOffset += block.UncompressedSize;
        }

        await Task.Run(() => Array.ConstrainedCopy(chunkData, 0, data, chunk.UncompressedOffset, chunk.Header.UncompressedSize));
      }

      return ByteArrayReader.CreateNew(data, start);
    }

    private async Task readNameTable(Action<DomainLoadProgress> progress) {
      DomainLoadProgress message = new DomainLoadProgress { Text = "Reading Name Table...", Current = 0, Total = NameTableCount };

      reader.Seek(NameTableOffset);

      for(int i = 0; i < NameTableCount; ++i) {
        DomainNameTableEntry name = new DomainNameTableEntry { TableIndex = i };

        await name.ReadNameTableEntry(reader);

        NameTable.Add(name);

        message.IncrementCurrent();

        if (NameTableCount > 100) progress?.Invoke(message);
      }
    }

    private async Task writeNameTable() {
      foreach(DomainNameTableEntry entry in NameTable) {
        await entry.WriteBuffer(writer, 0);
      }
    }

    private async Task readImportTable(Action<DomainLoadProgress> progress) {
      DomainLoadProgress message = new DomainLoadProgress { Text = "Reading Import Table...", Current = 0, Total = ImportTableCount };

      reader.Seek(ImportTableOffset);

      for(int i = 0; i < ImportTableCount; ++i) {
        DomainImportTableEntry import = new DomainImportTableEntry { TableIndex = -(i + 1) };

        await import.ReadImportTableEntry(reader, this);

        ImportTable.Add(import);

        message.IncrementCurrent();

        if (ImportTableCount > 100) progress?.Invoke(message);
      }

      message.Text    = "Expanding References...";
      message.Current = 0;
      message.Total   = 0;

      progress?.Invoke(message);

      await ImportTable.ForEachAsync(import => Task.Run(() => import.ExpandReferences(this)));
    }

    private async Task writeImportTable() {
      foreach(DomainImportTableEntry entry in ImportTable) {
        await entry.WriteBuffer(writer, 0);
      }
    }

    private async Task readExportTable(Action<DomainLoadProgress> progress) {
      DomainLoadProgress message = new DomainLoadProgress { Text = "Reading Export Table...", Current = 0, Total = ExportTableCount };

      reader.Seek(ExportTableOffset);

      for(int i = 0; i < ExportTableCount; ++i) {
        DomainExportTableEntry export = new DomainExportTableEntry { TableIndex = i + 1 };

        await export.ReadExportTableEntry(reader, this);

        ExportTable.Add(export);

        message.IncrementCurrent();

        if (ExportTableCount > 100) progress?.Invoke(message);
      }

      message.Text    = "Expanding References...";
      message.Current = 0;
      message.Total   = 0;

      progress?.Invoke(message);

      await ExportTable.ForEachAsync(export => Task.Run(() => export.ExpandReferences(this)));
    }

    private async Task writeExportTable() {
      foreach(DomainExportTableEntry entry in ExportTable) {
        await entry.WriteBuffer(writer, 0);
      }
    }

    private async Task readDependsTable() {
      reader.Seek(DependsTableOffset);

      DependsTable = await reader.ReadBytes(ExportTableCount * sizeof(uint));
    }

    private async Task writeDependsTable() {
      byte[] bytes = Enumerable.Repeat((byte)0, ExportTable.Count * sizeof(uint)).ToArray();

      await writer.WriteBytes(bytes);
    }

    #region External Code

    /// <summary>
    /// https://github.com/gildor2/UModel/blob/c871f9d534e0bd42a17b4d4268c0ecc59dd7191e/Unreal/UnPackage.cpp#L1274
    /// </summary>
    private async Task decodePointers() {
      uint code1 = (((uint)Size             & 0xffu) << 24)
                 | (((uint)NameTableCount   & 0xffu) << 16)
                 | (((uint)NameTableOffset  & 0xffu) << 8)
                 |  ((uint)ExportTableCount & 0xffu);

      int code2 = (ExportTableOffset + ImportTableCount + ImportTableOffset) & 0x1f;

      await Task.Run(() => {
        for(int i = 0; i < ExportTable.Count; ++i) ExportTable[i].DecodePointer(code1, code2, i);
      });
    }

    private async Task encodePointers() {
      uint code1 = (((uint)BuilderSize             & 0xffu) << 24)
                 | (((uint)NameTable.Count         & 0xffu) << 16)
                 | (((uint)BuilderNameTableOffset  & 0xffu) << 8)
                 |  ((uint)ExportTable.Count       & 0xffu);

      int code2 = (BuilderExportTableOffset + ImportTable.Count + BuilderImportTableOffset) & 0x1f;

      await Task.Run(() => {
        for(int i = 0; i < ExportTable.Count; ++i) ExportTable[i].EncodePointer(code1, code2, i);
      });
    }

    #endregion External Code

    #endregion Private Methods

  }

}
