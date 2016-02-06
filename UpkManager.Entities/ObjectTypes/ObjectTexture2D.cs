using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UpkManager.Entities.Compression;
using UpkManager.Entities.Constants;
using UpkManager.Entities.ObjectTypes.Texture2D;
using UpkManager.Entities.PropertyTypes;
using UpkManager.Entities.Tables;


namespace UpkManager.Entities.ObjectTypes {

  public class ObjectTexture2D : ObjectBase {

    #region Properties

    public byte[] Unknown1 { get; set; }

    public int CompressedChunkOffset { get; set; }

    public int CompressedChunkCount { get; set; }

    public List<MipMap> MipMaps { get; set; }

    public byte[] Guid { get; set; }

    #endregion Properties

    #region Overrides

    public override ObjectType ObjectType => ObjectType.Texture2D;

    public override bool CanObjectSave => true;

    public override void ReadUpkObject(byte[] data, ref int index, int endOffset, bool skipProperties, bool skipParse, List<NameTableEntry> nameTable) {
      base.ReadUpkObject(data, ref index, endOffset, skipProperties, skipParse, nameTable);

      if (skipParse) return;

      const int unknownSize = 3 * sizeof(uint);

      Unknown1 = new byte[unknownSize];

      Array.ConstrainedCopy(data, index, Unknown1, 0, unknownSize); index += unknownSize;

      CompressedChunkOffset = BitConverter.ToInt32(data, index); index += sizeof(int);
      CompressedChunkCount  = BitConverter.ToInt32(data, index); index += sizeof(int);

      MipMaps = new List<MipMap>();

      for(int i = 0; i < CompressedChunkCount; ++i) {
        CompressedChunkBulkData bulkChunk = new CompressedChunkBulkData();

        bulkChunk.ReadCompressedChunk(data, ref index);

        MipMap mip = new MipMap();

        mip.Width  = BitConverter.ToInt32(data, index); index += sizeof(int);
        mip.Height = BitConverter.ToInt32(data, index); index += sizeof(int);

        mip.ImageData = bulkChunk.DecompressChunk(0);

        MipMaps.Add(mip);
      }

      Guid = new byte[16];

      Array.ConstrainedCopy(data, index, Guid, 0, 16);

      index += 16;
    }

    public override void SaveObject(string filename) {
      if (MipMaps == null || !MipMaps.Any()) return;

      PropertyByteValue formatProp = PropertyHeader.GetProperty("Format").FirstOrDefault()?.Value as PropertyByteValue;

      if (formatProp == null) return;

      MipMap mipMap = MipMaps.Where(mm => mm.ImageData != null && mm.ImageData.Length > 0).OrderByDescending(mm => mm.Width > mm.Height ? mm.Width : mm.Height).FirstOrDefault();

      if (mipMap == null) return;

      string format = formatProp.ToString().Replace("PF_", null);

      DdsHeader header = new DdsHeader {
        Flags  = 0x00001007,
        Height = mipMap.Height,
        Width  = mipMap.Width,
        PixelFormat = new DdsPixelFormat {
          Flags = 0x00000004,
          FourCc = format
        },
        Caps = 0x00001000
      };

      FileStream stream = new FileStream(filename, FileMode.Create);

      stream.Write(header.GetDdsHeaderBytes(), 0, 128);

      stream.Write(mipMap.ImageData, 0, mipMap.ImageData.Length);

      stream.Close();
    }

    public override Stream GetObjectStream() {
      if (MipMaps == null || !MipMaps.Any()) return null;

      PropertyByteValue formatProp = PropertyHeader.GetProperty("Format").FirstOrDefault()?.Value as PropertyByteValue;

      if (formatProp == null) return null;

      string format = formatProp.ToString().Replace("PF_", null);

      if (!format.StartsWith("DXT")) return null;

      MipMap mipMap = MipMaps.Where(mm => mm.ImageData != null && mm.ImageData.Length > 0).OrderByDescending(mm => mm.Width > mm.Height ? mm.Width : mm.Height).FirstOrDefault();

      if (mipMap == null) return null;

      DdsHeader header = new DdsHeader {
        Flags  = 0x00001007,
        Height = mipMap.Height,
        Width  = mipMap.Width,
        PixelFormat = new DdsPixelFormat {
          Flags = 0x00000004,
          FourCc = format
        },
        Caps = 0x00001000
      };

      MemoryStream stream = new MemoryStream();

      stream.Write(header.GetDdsHeaderBytes(), 0, 128);

      stream.Write(mipMap.ImageData, 0, mipMap.ImageData.Length);

      stream.Flush();

      stream.Position = 0;

      return stream;
    }

    #endregion Overrides

  }

}
