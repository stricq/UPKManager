using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using CSharpImageLibrary.General;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Extensions;
using UpkManager.Domain.Helpers;
using UpkManager.Domain.Models.UpkFile.Properties;
using UpkManager.Domain.Models.UpkFile.Tables;


namespace UpkManager.Domain.Models.UpkFile.Objects.Textures {

  public sealed class DomainObjectTexture2D : DomainObjectCompressionBase {

    #region Constructor

    public DomainObjectTexture2D() {
      MipMaps = new List<DomainMipMap>();
    }

    #endregion Constructor

    #region Properties

    public int MipMapsCount { get; private set; }

    public List<DomainMipMap> MipMaps { get; }

    public byte[] Guid { get; private set; }

    #endregion Properties

    #region Domain Properties

    public override bool IsExportable => true;

    public override ViewableTypes Viewable => ViewableTypes.Image;

    public override ObjectTypes ObjectType => ObjectTypes.Texture2D;

    public override string FileExtension => ".dds";

    public override string FileTypeDesc => "Direct Draw Surface";

    #endregion Domain Properties

    #region Domain Methods

    public override async Task ReadDomainObject(ByteArrayReader reader, DomainHeader header, DomainExportTableEntry export, bool skipProperties, bool skipParse) {
      await base.ReadDomainObject(reader, header, export, skipProperties, skipParse);

      if (skipParse) return;

      MipMapsCount = reader.ReadInt32();

      for(int i = 0; i < MipMapsCount; ++i) {
        await ProcessCompressedBulkData(reader, async bulkChunk => {
          DomainMipMap mip = new DomainMipMap {
            Width  = reader.ReadInt32(),
            Height = reader.ReadInt32()
          };

          if (mip.Width >= 4 || mip.Height >= 4) mip.ImageData = (await bulkChunk.DecompressChunk(0))?.GetBytes();

          MipMaps.Add(mip);
        });
      }

      Guid = await reader.ReadBytes(16);
    }

    public override async Task SaveObject(string filename) {
      if (MipMaps == null || !MipMaps.Any()) return;

      ImageEngineFormat format;

      DomainMipMap mipMap = MipMaps.Where(mm => mm.ImageData != null && mm.ImageData.Length > 0).OrderByDescending(mm => mm.Width > mm.Height ? mm.Width : mm.Height).FirstOrDefault();

      if (mipMap == null) return;

      MemoryStream memory = buildDdsImage(MipMaps.IndexOf(mipMap), out format);

      if (memory == null) return;

      ImageEngineImage ddsImage = new ImageEngineImage(memory);

      FileStream stream = new FileStream(filename, FileMode.Create);

      await Task.Run(() => ddsImage.Save(stream, format, MipHandling.KeepTopOnly));

      stream.Close();

      memory.Close();
    }

    public override async Task SetObject(string filename, List<DomainNameTableEntry> nameTable) {
      ImageEngineImage image = await Task.Run(() => new ImageEngineImage(filename));

      int width  = image.Width;
      int height = image.Height;

      DomainPropertyIntValue sizeX = PropertyHeader.GetProperty("SizeX").FirstOrDefault()?.Value as DomainPropertyIntValue;
      DomainPropertyIntValue sizeY = PropertyHeader.GetProperty("SizeY").FirstOrDefault()?.Value as DomainPropertyIntValue;

      sizeX?.SetPropertyValue(width);
      sizeY?.SetPropertyValue(height);

      DomainPropertyIntValue mipTailBaseIdx = PropertyHeader.GetProperty("MipTailBaseIdx").FirstOrDefault()?.Value as DomainPropertyIntValue;

      mipTailBaseIdx?.SetPropertyValue((int)Math.Log(width > height ? width : height, 2));

      DomainPropertyStringValue filePath = PropertyHeader.GetProperty("SourceFilePath").FirstOrDefault()?.Value as DomainPropertyStringValue;
      DomainPropertyStringValue fileTime = PropertyHeader.GetProperty("SourceFileTimestamp").FirstOrDefault()?.Value as DomainPropertyStringValue;

      filePath?.SetPropertyValue(filename);
      fileTime?.SetPropertyValue(File.GetLastWriteTime(filename).ToString("yyyy-MM-dd hh:mm:ss"));

      DomainPropertyByteValue pfFormat = PropertyHeader.GetProperty("Format").FirstOrDefault()?.Value as DomainPropertyByteValue;

      ImageEngineFormat imageFormat = image.Format.InternalFormat;

      if (!imageFormat.ToString().Contains("DDS")) throw new Exception($"Image is not in a DDS format.  It is actually {imageFormat}.");

      if (pfFormat != null) {
        string formatStr =  imageFormat.ToString().Replace("DDS", "PF");

        DomainNameTableEntry formatTableEntry = nameTable.SingleOrDefault(nt => nt.Name.String == formatStr) ?? nameTable.AddDomainNameTableEntry(formatStr);

        pfFormat.SetPropertyValue(formatTableEntry);
      }

      MipMaps.Clear();

      while(true) {
        MemoryStream stream = new MemoryStream();

        image.Save(stream, imageFormat, MipHandling.KeepTopOnly);

        await stream.FlushAsync();

        MipMaps.Add(new DomainMipMap {
          ImageData = (await ByteArrayReader.CreateNew(stream.ToArray(), 0x80).Splice()).GetBytes(), // Strip off 128 bytes for the DDS header
          Width     = image.Width,
          Height    = image.Height
        });

        if (width == 1 && height == 1) break;

        if (width  > 1) width  /= 2;
        if (height > 1) height /= 2;

        if (image.Width > 4 && image.Height > 4) image.Resize(0.5, false);
      }
    }

    public override Stream GetObjectStream() {
      if (MipMaps == null || !MipMaps.Any()) return null;

      ImageEngineFormat format;

      DomainMipMap mipMap = MipMaps.Where(mm => mm.ImageData != null && mm.ImageData.Length > 0).OrderByDescending(mm => mm.Width > mm.Height ? mm.Width : mm.Height).FirstOrDefault();

      return mipMap == null ? null : buildDdsImage(MipMaps.IndexOf(mipMap), out format);
    }

    public Stream GetObjectStream(int mipMapIndex) {
      ImageEngineFormat format;

      return buildDdsImage(mipMapIndex, out format);
    }

    #endregion Domain Methods

    #region DomainUpkBuilderBase Implementation

    public override int GetBuilderSize() {
      BuilderSize = PropertyHeader.GetBuilderSize()
                  + base.GetBuilderSize()
                  + sizeof(int);

      foreach(DomainMipMap mipMap in MipMaps) {
        BulkDataCompressionTypes flags = mipMap.ImageData == null || mipMap.ImageData.Length == 0 ? BulkDataCompressionTypes.Unused | BulkDataCompressionTypes.StoreInSeparatefile : BulkDataCompressionTypes.LZO_ENC;

        BuilderSize += Task.Run(() => ProcessUncompressedBulkData(ByteArrayReader.CreateNew(mipMap.ImageData, 0), flags)).Result
                    +  sizeof(int) * 2;
      }

      BuilderSize += Guid.Length;

      return BuilderSize;
    }

    public override async Task WriteBuffer(ByteArrayWriter Writer, int CurrentOffset) {
      await PropertyHeader.WriteBuffer(Writer, CurrentOffset);

      await base.WriteBuffer(Writer, CurrentOffset);

      Writer.WriteInt32(MipMaps.Count);

      for(int i = 0; i < MipMaps.Count; ++i) {
        await CompressedChunks[i].WriteCompressedChunk(Writer, CurrentOffset);

        Writer.WriteInt32(MipMaps[i].Width);
        Writer.WriteInt32(MipMaps[i].Height);
      }

      await Writer.WriteBytes(Guid);
    }

    #endregion DomainUpkBuilderBase Implementation

    #region Private Methods

    private MemoryStream buildDdsImage(int mipMapIndex, out ImageEngineFormat imageFormat) {
      DomainPropertyByteValue formatProp = PropertyHeader.GetProperty("Format").FirstOrDefault()?.Value as DomainPropertyByteValue;

      imageFormat = ImageEngineFormat.Unknown;

      if (formatProp == null) return null;

      string format = formatProp.PropertyString.Replace("PF_", null);

      switch(format) {
        case "DXT1": {
          imageFormat = ImageEngineFormat.DDS_DXT1;

          break;
        }
        case "DXT5": {
          imageFormat = ImageEngineFormat.DDS_DXT5;

          break;
        }
        case "G8": {
          imageFormat = ImageEngineFormat.DDS_G8_L8;

          break;
        }
        case "A8R8G8B8": {
          imageFormat = ImageEngineFormat.DDS_ARGB;

          break;
        }
        default: {
          return null;
        }
      }

      DomainMipMap mipMap = MipMaps[mipMapIndex];

      DDSGeneral.DDS_HEADER header = DDSGeneral.Build_DDS_Header(0, mipMap.Height, mipMap.Width, imageFormat);

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
