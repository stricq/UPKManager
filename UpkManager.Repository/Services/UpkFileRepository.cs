using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using STR.Common.Contracts;
using STR.Common.Extensions;

using UpkManager.Domain.Contracts;
using UpkManager.Domain.Models;
using UpkManager.Domain.Models.Tables;
using UpkManager.Domain.Services;

using UpkManager.Entities;
using UpkManager.Entities.Compression;
using UpkManager.Entities.Tables;

using UpkManager.Repository.Constants;
using UpkManager.Repository.Contracts;


namespace UpkManager.Repository.Services {

  [Export(typeof(IUpkFileRepository))]
  public class UpkFileRepository : IUpkFileRepository {

    #region Private Fields

    private readonly IClassFactory classFactory;

    private readonly IMapper mapper;

    private readonly ILzoCompressor lzoCompressor;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public UpkFileRepository(IClassFactory ClassFactory, IMapper Mapper, ILzoCompressor LzoCompressor) {
      classFactory = ClassFactory;

      mapper = Mapper;

      lzoCompressor = LzoCompressor;
    }

    #endregion Constructor

    #region IUpkFileRepository Implementation

    public async Task<DomainHeader> LoadUpkFile(string filename) {
      IByteArrayReader reader = classFactory.Create<ByteArrayReader>();

      byte[] data = await Task.Run(() => File.ReadAllBytes(filename));

      reader.Initialize(data, 0);

      DomainHeader header = new DomainHeader(reader) {
        FullFilename = filename,
        FileSize     = data.LongLength
      };

      return header;
    }

    public async Task<DomainHeader> LoadAndParseUpk(string filename, bool SkipProperties, bool SkipParsing, Action<DomainLoadProgress> LoadProgress) {
      DomainLoadProgress message = new DomainLoadProgress { Text = "Loading File..." };

      LoadProgress?.Invoke(message);

      DomainHeader header = new DomainHeader(null);

      byte[] data = await Task.Run(() => File.ReadAllBytes(filename));

      header.FullFilename = filename;
      header.FileSize     = data.LongLength;

      message.Text = "Parsing Header...";

      LoadProgress?.Invoke(message);

      UpkHeader upkHeader = new UpkHeader();

      byte[] data1 = data;

      await Task.Run(() => upkHeader.ReadUpkHeader(data1));

      if (upkHeader.CompressedChunks.Count > 0) {
        if ((upkHeader.CompressionFlags & (CompressionFlag.LZO | CompressionFlag.LZO_ENC)) == 0) {
          message.IsComplete = true;

          LoadProgress?.Invoke(message);

          mapper.Map(upkHeader, header);

          return header;
        }

        data = await decompressChunksAsync(upkHeader.CompressedChunks, upkHeader.CompressionFlags, LoadProgress);
      }

      await readNameTable(data, upkHeader, LoadProgress);

      await readImportTable(data, upkHeader, LoadProgress);
      await readExportTable(data, upkHeader, LoadProgress);

      await Task.Run(() => readDependsTable(data, upkHeader));

      await Task.Run(() => patchPointers(upkHeader));

      await parseExportTableObjects(data, upkHeader, SkipProperties, SkipParsing, LoadProgress);

      mapper.Map(upkHeader, header); // Can't throw this on the background as Header is being observed.  Need to fix.

      message.IsComplete = true;

      LoadProgress?.Invoke(message);

      return header;
    }

    public async Task SaveObject(DomainExportTableEntry Export, string Filename) {
      ExportTableEntry export = mapper.Map<ExportTableEntry>(Export);

      await Task.Run(() => export.UpkObject.SaveObject(Filename));
    }

    public Stream GetObjectStream(DomainExportTableEntry Export) {
      ExportTableEntry export = mapper.Map<ExportTableEntry>(Export);

      return export.UpkObject.GetObjectStream();
    }

    #endregion IUpkFileRepository Implementation

    #region Private Methods

    private static async Task readNameTable(byte[] data, UpkHeader header, Action<DomainLoadProgress> loadProgress) {
      DomainLoadProgress message = new DomainLoadProgress { Text = "Reading Name Table", Current = 0, Total = header.NameTableCount };

      loadProgress?.Invoke(message);

      int index = header.NameTableOffset;

      for(int i = 0; i < header.NameTableCount; ++i) {
        NameTableEntry name = new NameTableEntry { TableIndex = i };

        await Task.Run(() => name.ReadNameTableEntry(data, ref index));

        header.NameTable.Add(name);

        if (header.NameTableCount > 1000) {
          message.Current += 1;

          loadProgress?.Invoke(message);
        }
      }
    }

    private static async Task readImportTable(byte[] data, UpkHeader header, Action<DomainLoadProgress> loadProgress) {
      DomainLoadProgress message = new DomainLoadProgress { Text = "Reading Import Table", Current = 0, Total = header.ImportTableCount };

      loadProgress?.Invoke(message);

      int index = header.ImportTableOffset;

      for(int i = 0; i < header.ImportTableCount; ++i) {
        ImportTableEntry import = new ImportTableEntry { TableIndex = -(i + 1) };

        int index1 = index;

        index = await Task.Run(() => import.ReadImportTableEntry(data, index1, header.NameTable));

        header.ImportTable.Add(import);

        if (header.ImportTableCount > 1000) {
          message.Current += 1;

          loadProgress?.Invoke(message);
        }
      }
    }

    private static async Task readExportTable(byte[] data, UpkHeader header, Action<DomainLoadProgress> loadProgress) {
      DomainLoadProgress message = new DomainLoadProgress { Text = "Reading Export Table", Current = 0, Total = header.ExportTableCount };

      loadProgress?.Invoke(message);

      int index = header.ExportTableOffset;

      for(int i = 0; i < header.ExportTableCount; ++i) {
        ExportTableEntry export = new ExportTableEntry { TableIndex = i + 1 };

        int index1 = index;

        index = await Task.Run(() => export.ReadExportTableEntry(data, index1, header.NameTable));

        header.ExportTable.Add(export);

        if (header.ExportTableCount > 1000) {
          message.Current += 1;

          loadProgress?.Invoke(message);
        }
      }
    }

    private static void readDependsTable(byte[] data, UpkHeader header) {
      header.DependsTable = new byte[header.Size - header.DependsTableOffset];

      Array.ConstrainedCopy(data, header.DependsTableOffset, header.DependsTable, 0, header.Size - header.DependsTableOffset);
    }

    private static async Task parseExportTableObjects(byte[] data, UpkHeader header, bool skipProperties, bool skipParse, Action<DomainLoadProgress> loadProgress) {
      DomainLoadProgress message = new DomainLoadProgress { Text = "Parsing Export Table Objects", Current = 0, Total = header.ExportTableCount };

      loadProgress?.Invoke(message);

      string msg;

      await header.ExportTable.ForEachAsync(export => {
        return Task.Run(() => {
          try {
            export.ReadObjectType(data, header, skipProperties, skipParse, out msg);

            if (!String.IsNullOrEmpty(msg)) {
              export.IsErrored             = true;
              export.ParseExceptionMessage = msg;

              header.IsErrored = true;

              export.ReadObjectType(data, header, true, true, out msg);
            }
          }
          catch(Exception ex) {
            export.IsErrored             = true;
            export.ParseExceptionMessage = ex.Message;

            header.IsErrored = true;

            export.ReadObjectType(data, header, true, true, out msg);
          }
        }).ContinueWith(task => {
          if (header.ExportTableCount > 1000) {
            message.IncrementCurrent();

            loadProgress?.Invoke(message);
          }
        });
      });
    }

    private async Task<byte[]> decompressChunksAsync(List<CompressedChunk> chunks, uint flags, Action<DomainLoadProgress> loadProgress) {
      DomainLoadProgress message = new DomainLoadProgress { Text = "Decompressing", Current = 0, Total = chunks.SelectMany(chunk => chunk.Header.Blocks).Count() };

      int totalSize = chunks.Min(ch => ch.UncompressedOffset);

      totalSize = chunks.SelectMany(ch => ch.Header.Blocks).Aggregate(totalSize, (total, block) => total + block.UncompressedSize);

      byte[] data = new byte[totalSize];

      foreach(CompressedChunk chunk in chunks) {
        byte[] chunkData = new byte[chunk.Header.Blocks.Sum(block => block.UncompressedSize)];

        int uncompressedOffset = 0;

        foreach(CompressedChunkBlock block in chunk.Header.Blocks) {
          if ((flags & CompressionFlag.LZO_ENC) == CompressionFlag.LZO_ENC) decryptChunk(block.CompressedData);

          byte[] decompressed = await lzoCompressor.DecompressAsync(block.CompressedData, block.UncompressedSize);

//        block.UncompressedOffset = chunk.UncompressedOffset + uncompressedOffset;

          int offset = uncompressedOffset;

          await Task.Run(() => Array.ConstrainedCopy(decompressed, 0, chunkData, offset, block.UncompressedSize));

          uncompressedOffset += block.UncompressedSize;

          message.Current += 1;

          loadProgress?.Invoke(message);
        }

        await Task.Run(() => Array.ConstrainedCopy(chunkData, 0, data, chunk.UncompressedOffset, chunk.Header.UncompressedSize));
      }

      return data;
    }
    //
    // Following four methods are from:
    //
    // https://github.com/gildor2/UModel/blob/c871f9d534e0bd42a17b4d4268c0ecc59dd7191e/Unreal/UnPackage.cpp
    //
    private static void patchPointers(UpkHeader header) {
      uint code1 = ((uint)header.Size             & 0xffu) << 24
                 | ((uint)header.NameTableCount   & 0xffu) << 16
                 | ((uint)header.NameTableOffset  & 0xffu) << 8
                 | ((uint)header.ExportTableCount & 0xffu);

      int code2 = (header.ExportTableOffset + header.ImportTableCount + header.ImportTableOffset) & 0x1f;

      List<ExportTableEntry> exports = header.ExportTable;

      for(int i = 0; i < exports.Count; ++i) {
        uint size   = (uint)exports[i].SerialDataSize;
        uint offset = (uint)exports[i].SerialDataOffset;

        decodePointer(ref size,   code1, code2, i);
        decodePointer(ref offset, code1, code2, i);

        exports[i].SerialDataSize   = (int)size;
        exports[i].SerialDataOffset = (int)offset;
      }
    }

    private static void decodePointer(ref uint value, uint code1, int code2, int index) {
      uint tmp1 = ror32(value, (index + code2) & 0x1f);
      uint tmp2 = ror32(code1, index % 32);

      value = tmp2 ^ tmp1;
    }

    private static uint ror32(uint val, int shift) {
      return (val >> shift) | (val << (32 - shift));
    }

    private static void decryptChunk(byte[] data) {
      if (data.Length < 32) return;

//    const string key = "qiffjdlerdoqymvketdcl0er2subioxq";

      byte[] key = { 0x71, 0x69, 0x66, 0x66, 0x6a, 0x64, 0x6c, 0x65, 0x72, 0x64, 0x6f, 0x71, 0x79, 0x6d, 0x76, 0x6b, 0x65, 0x74, 0x64, 0x63, 0x6c, 0x30, 0x65, 0x72, 0x32, 0x73, 0x75, 0x62, 0x69, 0x6f, 0x78, 0x71 };

      for(int i = 0; i < data.Length; ++i) data[i] ^= key[i % 32];
    }

    #endregion Private Methods

  }

}
