using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using CSharpImageLibrary.General;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Helpers;
using UpkManager.Domain.Models.Objects.Texture2D;
using UpkManager.Domain.Models.Properties;
using UpkManager.Domain.Models.Tables;


namespace UpkManager.Domain.Models.Objects {

  public class DomainObjectTexture2D : DomainObjectCompressionBase {

    #region Constructor

    public DomainObjectTexture2D() {
      MipMaps = new List<DomainMipMap>();
    }

    #endregion Constructor

    #region Properties

    public int MipMapsCount { get; set; }

    public List<DomainMipMap> MipMaps { get; set; }

    public byte[] Guid { get; set; }

    public override bool IsExportable => true;

    public override bool IsViewable => true;

    public override ObjectType ObjectType => ObjectType.Texture2D;

    #endregion Properties

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

          if (mip.Width >= 4 && mip.Height >= 4) mip.ImageData = (await bulkChunk.DecompressChunk(0))?.GetByteArray();

          MipMaps.Add(mip);
        });
      }

      Guid = await reader.ReadBytes(16);
    }

    public override async Task SaveObject(string filename) {
      if (MipMaps == null || !MipMaps.Any()) return;

      ImageEngineFormat format;

      MemoryStream memory = buildDdsImage(out format);

      if (memory == null) return;

      ImageEngineImage ddsImage = new ImageEngineImage(memory);

      FileStream stream = new FileStream(filename, FileMode.Create);

      await Task.Run(() => ddsImage.Save(stream, format, MipHandling.KeepTopOnly));

      stream.Close();

      memory.Close();
    }

    public override Stream GetObjectStream() {
      if (MipMaps == null || !MipMaps.Any()) return null;

      ImageEngineFormat format;

      return buildDdsImage(out format);
    }

    #endregion Domain Methods

    #region Private Methods

    private MemoryStream buildDdsImage(out ImageEngineFormat imageFormat) {
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

      DomainMipMap mipMap = MipMaps.Where(mm => mm.ImageData != null && mm.ImageData.Length > 0).OrderByDescending(mm => mm.Width > mm.Height ? mm.Width : mm.Height).FirstOrDefault();

      if (mipMap == null) return null;

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
