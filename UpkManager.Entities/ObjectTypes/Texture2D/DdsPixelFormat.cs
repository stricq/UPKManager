

using System;
using System.Text;


namespace UpkManager.Entities.ObjectTypes.Texture2D {

  public class DdsPixelFormat {

    #region Constructor

    public DdsPixelFormat() {
      Size = 32;
    }

    #endregion Constructor

    #region Properties

    public int Size { get; set; }

    public uint Flags { get; set; }

    public string FourCc { get; set; }

    public int RgbBitCount { get; set; }

    public uint RBitMask { get; set; }

    public uint GBitMask { get; set; }

    public uint BBitMask { get; set; }

    public uint ABitMask { get; set; }

    #endregion Properties

    #region Public Methods

    public byte[] GetDdsPixelFormatBytes() {
      byte[] data = new byte[Size];

      byte[] fourcc = Encoding.ASCII.GetBytes(FourCc);

      int index = 0;

      Array.ConstrainedCopy(BitConverter.GetBytes(Size), 0, data, index, sizeof(int)); index += sizeof(int);

      Array.ConstrainedCopy(BitConverter.GetBytes(Flags), 0, data, index, sizeof(uint)); index += sizeof(uint);

      Array.ConstrainedCopy(fourcc, 0, data, index, fourcc.Length); index += fourcc.Length;

      Array.ConstrainedCopy(BitConverter.GetBytes(RgbBitCount), 0, data, index, sizeof(uint)); index += sizeof(uint);

      Array.ConstrainedCopy(BitConverter.GetBytes(RBitMask), 0, data, index, sizeof(uint)); index += sizeof(uint);
      Array.ConstrainedCopy(BitConverter.GetBytes(GBitMask), 0, data, index, sizeof(uint)); index += sizeof(uint);
      Array.ConstrainedCopy(BitConverter.GetBytes(BBitMask), 0, data, index, sizeof(uint)); index += sizeof(uint);
      Array.ConstrainedCopy(BitConverter.GetBytes(ABitMask), 0, data, index, sizeof(uint));

      return data;
    }

    #endregion Public Methods

  }

}
