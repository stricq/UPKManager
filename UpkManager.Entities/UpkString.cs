using System;
using System.Text;


namespace UpkManager.Entities {

  public class UpkString {

    #region Properties

    public int Size { get; set; }

    public string String { get; set; }

    #endregion Properties

    #region Overrides

    public override string ToString() {
      return String;
    }

    #endregion Overrides

    #region Public Methods

    public void ReadUpkStr(byte[] data, ref int index) {
      Size = BitConverter.ToInt32(data, index); index += sizeof(int);

      if (Size == 0) {
        String = String.Empty;

        return;
      }

      if (Size < 0) {
        Size = -Size * 2;

        byte[] str = new byte[Size];

        Array.ConstrainedCopy(data, index, str, 0, Size);

        index += Size;

        String = Encoding.Unicode.GetString(str);
      }
      else {
        byte[] str = new byte[Size - 1];

        Array.ConstrainedCopy(data, index, str, 0, Size - 1);

        index += Size;

        String = Encoding.ASCII.GetString(str);
      }
    }

    #endregion Public Methods

  }

}
