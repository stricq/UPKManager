using System;
using System.Text;


namespace UpkManager.Entities.ObjectTypes.Texture2D {

  public class DdsHeader {

    #region Constructor

    public DdsHeader() {
      Size = 124;

      Reserved1 = new byte[11 * sizeof(uint)];
    }

    #endregion Constructor

    #region Properties

    public int Size { get; set; }

    public uint Flags { get; set; }

    public int Height { get; set; }

    public int Width { get; set; }

    public int PitchOrLinearSize { get; set; }

    public int Depth { get; set; }

    public int MipMapCount { get; set; }

    public byte[] Reserved1 { get; set; }

    public DdsPixelFormat PixelFormat { get; set; }

    public uint Caps { get; set; }

    public uint Caps2 { get; set; }

    public uint Caps3 { get; set; }

    public uint Caps4 { get; set; }

    public uint Reserved2 { get; set; }

    #endregion Properties

    #region Public Methods

    public byte[] GetDdsHeaderBytes() {
      byte[] data = new byte[128];

      const string magic = "DDS ";

      int index = 0;

      Array.ConstrainedCopy(Encoding.ASCII.GetBytes(magic), 0, data, index, magic.Length); index += magic.Length;

      Array.ConstrainedCopy(BitConverter.GetBytes(Size), 0, data, index, sizeof(int)); index += sizeof(int);

      Array.ConstrainedCopy(BitConverter.GetBytes(Flags), 0, data, index, sizeof(uint)); index += sizeof(uint);

      Array.ConstrainedCopy(BitConverter.GetBytes(Height), 0, data, index, sizeof(int)); index += sizeof(int);
      Array.ConstrainedCopy(BitConverter.GetBytes(Width),  0, data, index, sizeof(int)); index += sizeof(int);

      Array.ConstrainedCopy(BitConverter.GetBytes(PitchOrLinearSize), 0, data, index, sizeof(int)); index += sizeof(int);

      Array.ConstrainedCopy(BitConverter.GetBytes(Depth), 0, data, index, sizeof(int)); index += sizeof(int);

      Array.ConstrainedCopy(BitConverter.GetBytes(MipMapCount), 0, data, index, sizeof(int)); index += sizeof(int);

      Array.ConstrainedCopy(Reserved1, 0, data, index, Reserved1.Length); index += Reserved1.Length;

      Array.ConstrainedCopy(PixelFormat.GetDdsPixelFormatBytes(), 0, data, index, PixelFormat.Size); index += PixelFormat.Size;

      Array.ConstrainedCopy(BitConverter.GetBytes(Caps),  0, data, index, sizeof(uint)); index += sizeof(uint);
      Array.ConstrainedCopy(BitConverter.GetBytes(Caps2), 0, data, index, sizeof(uint)); index += sizeof(uint);
      Array.ConstrainedCopy(BitConverter.GetBytes(Caps3), 0, data, index, sizeof(uint)); index += sizeof(uint);
      Array.ConstrainedCopy(BitConverter.GetBytes(Caps4), 0, data, index, sizeof(uint)); index += sizeof(uint);

      Array.ConstrainedCopy(BitConverter.GetBytes(Reserved2), 0, data, index, sizeof(uint));

      return data;
    }

    #endregion Public Methods

  }

}
