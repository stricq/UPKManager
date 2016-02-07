using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using CSharpImageLibrary.General;

using UpkManager.Entities.Compression;
using UpkManager.Entities.Constants;
using UpkManager.Entities.ObjectTypes.Texture2D;
using UpkManager.Entities.PropertyTypes;
using UpkManager.Entities.Tables;

using MipMap = UpkManager.Entities.ObjectTypes.Texture2D.MipMap;


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

        if (mip.Width >= 4 && mip.Height >= 4) mip.ImageData = bulkChunk.DecompressChunk(0);

        MipMaps.Add(mip);
      }

      Guid = new byte[16];

      Array.ConstrainedCopy(data, index, Guid, 0, 16);

      index += 16;
    }

    public override void SaveObject(string filename) {
      if (MipMaps == null || !MipMaps.Any()) return;

      MemoryStream memory = buildHeader();

      if (memory == null) return;

      byte[] image = memory.GetBuffer();

      FileStream stream = new FileStream(filename, FileMode.Create);

      stream.Write(image, 0, image.Length);

      stream.Close();
    }

    public override Stream GetObjectStream() {
      if (MipMaps == null || !MipMaps.Any()) return null;

      return buildHeader();
    }

    #endregion Overrides

    #region Private Methods

    private MemoryStream buildHeader() {
      PropertyByteValue formatProp = PropertyHeader.GetProperty("Format").FirstOrDefault()?.Value as PropertyByteValue;

      if (formatProp == null) return null;

      string format = formatProp.ToString().Replace("PF_", null);

      ImageEngineFormat fourcc;

      switch(format) {
        case "DXT1": {
          fourcc = ImageEngineFormat.DDS_DXT1;

          break;
        }
        case "DXT5": {
          fourcc = ImageEngineFormat.DDS_DXT5;

          break;
        }
        case "G8": {
          fourcc = ImageEngineFormat.DDS_G8_L8;

          break;
        }
        case "A8R8G8B8": {
          fourcc = ImageEngineFormat.DDS_ARGB;

          break;
        }
        default: {
          return null;
        }
      }

      MipMap mipMap = MipMaps.Where(mm => mm.ImageData != null && mm.ImageData.Length > 0).OrderByDescending(mm => mm.Width > mm.Height ? mm.Width : mm.Height).FirstOrDefault();

      if (mipMap == null) return null;

      DDSGeneral.DDS_HEADER header = DDSGeneral.Build_DDS_Header(0, mipMap.Height, mipMap.Width, fourcc);

      MemoryStream stream = new MemoryStream();

      BinaryWriter writer = new BinaryWriter(stream);

      DDSGeneral.Write_DDS_Header(header, writer);

      stream.Write(mipMap.ImageData, 0, mipMap.ImageData.Length);

      stream.Flush();

      stream.Position = 0;

      return stream;
    }

    #endregion Private Methods

  }

}
