using System.IO;

using UpkManager.Dds.Constants;


namespace UpkManager.Dds {

  public sealed class DdsHeader {

    #region Constructor

    internal DdsHeader() { }

    public DdsHeader(FileFormat format, int width, int height) {
      PixelFormat = new DdsPixelFormat(format);

      Width  = (uint)width;
      Height = (uint)height;

      Size = 18 * 4 + PixelFormat.Size + 5 * 4;

      HeaderFlags  = (uint)Constants.HeaderFlags.Texture;
      SurfaceFlags = (uint)Constants.SurfaceFlags.Texture;

      MipMapCount = 1;
    }

    #endregion Constructor

    #region Public Properties

    public uint	Size { get; private set; }

    public uint	HeaderFlags { get; private set; }

    public uint	Height { get; private set; }

    public uint	Width { get; private set; }

    public uint	PitchOrLinearSize { get; private set; }

    public uint	Depth { get; private set; }

    public uint	MipMapCount { get; private set; }

    public uint	Reserved1_0 { get; private set; }

    public uint	Reserved1_1 { get; private set; }

    public uint	Reserved1_2 { get; private set; }

    public uint	Reserved1_3 { get; private set; }

    public uint	Reserved1_4 { get; private set; }

    public uint	Reserved1_5 { get; private set; }

    public uint	Reserved1_6 { get; private set; }

    public uint	Reserved1_7 { get; private set; }

    public uint	Reserved1_8 { get; private set; }

    public uint	Reserved1_9 { get; private set; }

    public uint	Reserved1_10 { get; private set; }

    public uint	SurfaceFlags { get; private set; }

    public uint	CubemapFlags { get; private set; }

    public uint	Reserved2_0 { get; private set; }

    public uint	Reserved2_1 { get; private set; }

    public uint	Reserved2_2 { get; private set; }

    public DdsPixelFormat	PixelFormat { get; private set; }

    #endregion Public Properties

    #region Public Methods

    public void Resize(int width, int height) {
      Width  = (uint)width;
      Height = (uint)height;
    }

    public void Read(BinaryReader reader) {
      Size              = reader.ReadUInt32();
      HeaderFlags       = reader.ReadUInt32();
      Height            = reader.ReadUInt32();
      Width             = reader.ReadUInt32();
      PitchOrLinearSize = reader.ReadUInt32();
      Depth             = reader.ReadUInt32();
      MipMapCount       = reader.ReadUInt32();
      Reserved1_0       = reader.ReadUInt32();
      Reserved1_1       = reader.ReadUInt32();
      Reserved1_2       = reader.ReadUInt32();
      Reserved1_3       = reader.ReadUInt32();
      Reserved1_4       = reader.ReadUInt32();
      Reserved1_5       = reader.ReadUInt32();
      Reserved1_6       = reader.ReadUInt32();
      Reserved1_7       = reader.ReadUInt32();
      Reserved1_8       = reader.ReadUInt32();
      Reserved1_9       = reader.ReadUInt32();
      Reserved1_10      = reader.ReadUInt32();

      PixelFormat = new DdsPixelFormat();

      PixelFormat.Read(reader);

      SurfaceFlags = reader.ReadUInt32();
      CubemapFlags = reader.ReadUInt32();
      Reserved2_0  = reader.ReadUInt32();
      Reserved2_1  = reader.ReadUInt32();
      Reserved2_2  = reader.ReadUInt32();
    }

    public void Write(BinaryWriter writer) {
      writer.Write(HeaderValues.DdsSignature); // "DDS "

      writer.Write(Size);
      writer.Write(HeaderFlags);
      writer.Write(Height);
      writer.Write(Width);
      writer.Write(PitchOrLinearSize);
      writer.Write(Depth);
      writer.Write(MipMapCount);
      writer.Write(Reserved1_0);
      writer.Write(Reserved1_1);
      writer.Write(Reserved1_2);
      writer.Write(Reserved1_3);
      writer.Write(Reserved1_4);
      writer.Write(Reserved1_5);
      writer.Write(Reserved1_6);
      writer.Write(Reserved1_7);
      writer.Write(Reserved1_8);
      writer.Write(Reserved1_9);
      writer.Write(Reserved1_10);

      PixelFormat.Write(writer);

      writer.Write(SurfaceFlags);
      writer.Write(CubemapFlags);
      writer.Write(Reserved2_0);
      writer.Write(Reserved2_1);
      writer.Write(Reserved2_2);

      writer.Flush();
    }

    #endregion Public Methods

  }

}
