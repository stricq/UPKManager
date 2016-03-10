using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using STR.Common.Extensions;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Helpers;
using UpkManager.Domain.Models.Compression;
using UpkManager.Domain.Models.Tables;


namespace UpkManager.Domain.Models {

  public class DomainHeader {

    #region Private Fields

    private ByteArrayReader reader;

    #endregion Private Fields

    #region Constructor

    public DomainHeader(ByteArrayReader Reader) {
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

      await patchPointers();

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

    public DomainObjectTableEntry GetObjectTableEntry(int reference) {
      if (reference == 0) return null;

      if (reference < 0 && -reference - 1 < ImportTableCount) return ImportTable[-reference - 1];
      if (reference > 0 &&  reference - 1 < ExportTableCount) return ExportTable[reference - 1];

      throw new Exception($"Object reference ({reference:X8}) is out of range of both the Import and Export Tables.");
    }

    #endregion Domain Methods

    #region Private Methods

    private async Task readUpkHeader() {
      reader.Seek(0);

      Signature = reader.ReadUInt32();

      if (Signature == FileHeader.EncryptedSignature) await reader.Decrypt();
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

    private async Task readDependsTable() {
      reader.Seek(DependsTableOffset);

      DependsTable = await reader.ReadBytes(Size - DependsTableOffset);
    }

    #region External Code

    /// <summary>
    /// https://github.com/gildor2/UModel/blob/c871f9d534e0bd42a17b4d4268c0ecc59dd7191e/Unreal/UnPackage.cpp#L1274
    /// </summary>
    private async Task patchPointers() {
      uint code1 = ((uint)Size             & 0xffu) << 24
                 | ((uint)NameTableCount   & 0xffu) << 16
                 | ((uint)NameTableOffset  & 0xffu) << 8
                 | ((uint)ExportTableCount & 0xffu);

      int code2 = (ExportTableOffset + ImportTableCount + ImportTableOffset) & 0x1f;

      await Task.Run(() => {
        for(int i = 0; i < ExportTable.Count; ++i) {
          uint size   = (uint)ExportTable[i].SerialDataSize;
          uint offset = (uint)ExportTable[i].SerialDataOffset;

          decodePointer(ref size,   code1, code2, i);
          decodePointer(ref offset, code1, code2, i);

          ExportTable[i].SerialDataSize   = (int)size;
          ExportTable[i].SerialDataOffset = (int)offset;
        }
      });
    }

    private static void decodePointer(ref uint value, uint code1, int code2, int index) {
      uint tmp1 = ror32(value, (index + code2) & 0x1f);
      uint tmp2 = ror32(code1, index % 32);

      value = tmp2 ^ tmp1;
    }

    private static uint ror32(uint val, int shift) {
      return (val >> shift) | (val << (32 - shift));
    }

    #endregion External Code

    #endregion Private Methods

  }

}
