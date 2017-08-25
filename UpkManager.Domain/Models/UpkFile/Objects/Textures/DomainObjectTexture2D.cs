using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using UpkManager.Dds;
using UpkManager.Dds.Constants;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Extensions;
using UpkManager.Domain.Helpers;
using UpkManager.Domain.Models.UpkFile.Properties;
using UpkManager.Domain.Models.UpkFile.Tables;


namespace UpkManager.Domain.Models.UpkFile.Objects.Textures {

  public class DomainObjectTexture2D : DomainObjectCompressionBase {

    #region Constructor

    internal DomainObjectTexture2D() {
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

    public override async Task SaveObject(string filename, object configuration) {
      if (MipMaps == null || !MipMaps.Any()) return;

      DdsSaveConfig config = configuration as DdsSaveConfig ?? new DdsSaveConfig(FileFormat.Unknown, 0, 0, false, false);

      FileFormat format;

      DomainMipMap mipMap = MipMaps.Where(mm => mm.ImageData != null && mm.ImageData.Length > 0).OrderByDescending(mm => mm.Width > mm.Height ? mm.Width : mm.Height).FirstOrDefault();

      if (mipMap == null) return;

      Stream memory = buildDdsImage(MipMaps.IndexOf(mipMap), out format);

      if (memory == null) return;

      DdsFile ddsImage = new DdsFile(memory);

      FileStream ddsStream = new FileStream(filename, FileMode.Create);

      config.FileFormat = format;

      await Task.Run(() => ddsImage.Save(ddsStream, config));

      ddsStream.Close();

      memory.Close();
    }

    public override async Task SetObject(string filename, List<DomainNameTableEntry> nameTable, object configuration) {
      DdsSaveConfig config = configuration as DdsSaveConfig ?? new DdsSaveConfig(FileFormat.Unknown, 0, 0, false, false);

      DdsFile image = await Task.Run(() => new DdsFile(filename));

      bool skipFirstMip = false;

      int width  = image.Width;
      int height = image.Height;

      if (MipMaps[0].ImageData == null || MipMaps[0].ImageData.Length == 0) {
        width  *= 2;
        height *= 2;

        skipFirstMip = true;
      }

      DomainPropertyIntValue sizeX = PropertyHeader.GetProperty("SizeX").FirstOrDefault()?.Value as DomainPropertyIntValue;
      DomainPropertyIntValue sizeY = PropertyHeader.GetProperty("SizeY").FirstOrDefault()?.Value as DomainPropertyIntValue;

      sizeX?.SetPropertyValue(skipFirstMip ? width  * 2 : width);
      sizeY?.SetPropertyValue(skipFirstMip ? height * 2 : height);

      DomainPropertyIntValue mipTailBaseIdx = PropertyHeader.GetProperty("MipTailBaseIdx").FirstOrDefault()?.Value as DomainPropertyIntValue;

      int indexSize = width > height ? width : height;

      mipTailBaseIdx?.SetPropertyValue((int)Math.Log(skipFirstMip ? indexSize * 2 : indexSize , 2));

      DomainPropertyStringValue filePath = PropertyHeader.GetProperty("SourceFilePath").FirstOrDefault()?.Value as DomainPropertyStringValue;
      DomainPropertyStringValue fileTime = PropertyHeader.GetProperty("SourceFileTimestamp").FirstOrDefault()?.Value as DomainPropertyStringValue;

      filePath?.SetPropertyValue(filename);
      fileTime?.SetPropertyValue(File.GetLastWriteTime(filename).ToString("yyyy-MM-dd hh:mm:ss"));

      DomainPropertyByteValue pfFormat = PropertyHeader.GetProperty("Format").FirstOrDefault()?.Value as DomainPropertyByteValue;

      FileFormat imageFormat = FileFormat.Unknown;

      if (pfFormat != null) imageFormat = DdsPixelFormat.ParseFileFormat(pfFormat.PropertyString);

      if (imageFormat == FileFormat.Unknown) throw new Exception($"Unknown DDS File Format ({pfFormat?.PropertyString ?? "Unknown"}).");

      if (config.FileFormat == FileFormat.Unknown) config.FileFormat = imageFormat;
      else {
        string formatStr = DdsPixelFormat.BuildFileFormat(config.FileFormat);

        DomainNameTableEntry formatTableEntry = nameTable.SingleOrDefault(nt => nt.Name.String == formatStr) ?? nameTable.AddDomainNameTableEntry(formatStr);

        pfFormat?.SetPropertyValue(formatTableEntry);
      }

      MipMaps.Clear();

      if (skipFirstMip) {
        MipMaps.Add(new DomainMipMap {
          ImageData = null,
          Width     = width,
          Height    = height
        });
      }

      image.GenerateMipMaps(4, 4);

      foreach(DdsMipMap mipMap in image.MipMaps.OrderByDescending(mip => mip.Width)) {
        MipMaps.Add(new DomainMipMap {
          ImageData = image.WriteMipMap(mipMap, config),
          Width     = mipMap.Width,
          Height    = mipMap.Height
        });
      }
    }

    public override Stream GetObjectStream() {
      if (MipMaps == null || !MipMaps.Any()) return null;

      FileFormat format;

      DomainMipMap mipMap = MipMaps.Where(mm => mm.ImageData != null && mm.ImageData.Length > 0).OrderByDescending(mm => mm.Width > mm.Height ? mm.Width : mm.Height).FirstOrDefault();

      return mipMap == null ? null : buildDdsImage(MipMaps.IndexOf(mipMap), out format);
    }

    public Stream GetObjectStream(int mipMapIndex) {
      FileFormat format;

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

    private Stream buildDdsImage(int mipMapIndex, out FileFormat imageFormat) {
      DomainPropertyByteValue formatProp = PropertyHeader.GetProperty("Format").FirstOrDefault()?.Value as DomainPropertyByteValue;

      imageFormat = FileFormat.Unknown;

      if (formatProp == null) return null;

      string format = formatProp.PropertyString;

      DomainMipMap mipMap = MipMaps[mipMapIndex];

      imageFormat = DdsPixelFormat.ParseFileFormat(format);

      DdsHeader ddsHeader = new DdsHeader(new DdsSaveConfig(imageFormat, 0, 0, false, false), mipMap.Width, mipMap.Height);

      MemoryStream stream = new MemoryStream();

      BinaryWriter writer = new BinaryWriter(stream);

      ddsHeader.Write(writer);

      stream.Write(mipMap.ImageData, 0, mipMap.ImageData.Length);

      stream.Flush();

      stream.Position = 0;

      return stream;
    }

    #endregion Private Methods

  }

}
