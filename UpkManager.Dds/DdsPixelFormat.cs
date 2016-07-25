using System.IO;

using UpkManager.Dds.Constants;


namespace UpkManager.Dds {

  public sealed class DdsPixelFormat {

    #region Constructor

    public DdsPixelFormat() { }

    public DdsPixelFormat(FileFormat fileFormat) {
      Size = 8 * 4;

      switch(fileFormat) {
        case FileFormat.DXT1:
        case FileFormat.DXT3:
        case FileFormat.DXT5: {
          Flags = (int)PixelFormatFlags.FourCC;

          RgbBitCount = 0;

          RBitMask = 0;
          GBitMask = 0;
          BBitMask = 0;
          ABitMask = 0;

          if (fileFormat == FileFormat.DXT1) FourCC = 0x31545844; // "DXT1"
          if (fileFormat == FileFormat.DXT3) FourCC = 0x33545844; // "DXT3"
          if (fileFormat == FileFormat.DXT5) FourCC = 0x35545844; // "DXT5"

          break;
        }
        case FileFormat.A8R8G8B8: {
          Flags = (int)PixelFormatFlags.RGBA;

          RgbBitCount = 32;

          FourCC = 0;

          RBitMask = 0x00ff0000;
          GBitMask = 0x0000ff00;
          BBitMask = 0x000000ff;
          ABitMask = 0xff000000;

          break;
        }
        case FileFormat.X8R8G8B8: {
          Flags = (int)PixelFormatFlags.RGB;

          RgbBitCount = 32;

          FourCC = 0;

          RBitMask = 0x00ff0000;
          GBitMask = 0x0000ff00;
          BBitMask = 0x000000ff;
          ABitMask = 0x00000000;

          break;
        }
        case FileFormat.A8B8G8R8: {
          Flags = (int)PixelFormatFlags.RGBA;

          RgbBitCount = 32;

          FourCC = 0;

          RBitMask = 0x000000ff;
          GBitMask = 0x0000ff00;
          BBitMask = 0x00ff0000;
          ABitMask = 0xff000000;

          break;
        }
        case FileFormat.X8B8G8R8: {
          Flags = (int)PixelFormatFlags.RGB;

          RgbBitCount = 32;

          FourCC = 0;

          RBitMask = 0x000000ff;
          GBitMask = 0x0000ff00;
          BBitMask = 0x00ff0000;
          ABitMask = 0x00000000;

          break;
        }
        case FileFormat.A1R5G5B5: {
          Flags = (int)PixelFormatFlags.RGBA;

          RgbBitCount = 16;

          FourCC = 0;

          RBitMask = 0x00007c00;
          GBitMask = 0x000003e0;
          BBitMask = 0x0000001f;
          ABitMask = 0x00008000;

          break;
        }
        case FileFormat.A4R4G4B4: {
          Flags = (int)PixelFormatFlags.RGBA;

          RgbBitCount = 16;

          FourCC = 0;

          RBitMask = 0x00000f00;
          GBitMask = 0x000000f0;
          BBitMask = 0x0000000f;
          ABitMask = 0x0000f000;

          break;
        }
        case FileFormat.R8G8B8: {
          Flags = (int)PixelFormatFlags.RGB;

          FourCC = 0;

          RgbBitCount = 24;

          RBitMask = 0x00ff0000;
          GBitMask = 0x0000ff00;
          BBitMask = 0x000000ff;
          ABitMask = 0x00000000;

          break;
        }
        case FileFormat.R5G6B5: {
          Flags = (int)PixelFormatFlags.RGB;

          FourCC = 0;

          RgbBitCount = 16;

          RBitMask = 0x0000f800;
          GBitMask = 0x000007e0;
          BBitMask = 0x0000001f;
          ABitMask = 0x00000000;

          break;
        }
      }
    }

    #endregion Constructor

    #region Public Properties

    public uint Size { get; private set; }

    public uint Flags { get; private set; }

    public uint FourCC { get; private set; }

    public uint RgbBitCount { get; private set; }

    public uint RBitMask { get; private set; }

    public uint GBitMask { get; private set; }

    public uint BBitMask { get; private set; }

    public uint ABitMask { get; private set; }

    #endregion Public Properties

    #region Public Methods

    internal void Read(BinaryReader reader) {
      Size			  = reader.ReadUInt32();
      Flags		    = reader.ReadUInt32();
      FourCC		  = reader.ReadUInt32();
      RgbBitCount	= reader.ReadUInt32();
      RBitMask		= reader.ReadUInt32();
      GBitMask		= reader.ReadUInt32();
      BBitMask		= reader.ReadUInt32();
      ABitMask		= reader.ReadUInt32();
    }

    internal void Write(BinaryWriter writer) {
      writer.Write(Size);
      writer.Write(Flags);
      writer.Write(FourCC);
      writer.Write(RgbBitCount);
      writer.Write(RBitMask);
      writer.Write(GBitMask);
      writer.Write(BBitMask);
      writer.Write(ABitMask);

      writer.Flush();
    }

    #endregion Public Methods

  }

}
