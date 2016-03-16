using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UpkManager.Domain.Helpers;


namespace UpkManager.Domain.Models {

  public class DomainString : DomainUpkBuilderBase {

    #region Properties

    public int Size { get; set; }

    public string String { get; set; }

    #endregion Properties

    #region Domain Methods

    public async Task ReadString(ByteArrayReader reader) {
      Size = reader.ReadInt32();

      if (Size == 0) {
        String = String.Empty;

        return;
      }

      if (Size < 0) {
        Size = -Size * 2;

        byte[] str = await reader.ReadBytes(Size);

        String = Encoding.Unicode.GetString(str);
      }
      else {
        byte[] str = await reader.ReadBytes(Size - 1);

        reader.Skip(1); // NULL Terminator

        String = Encoding.ASCII.GetString(str);
      }
    }

    #endregion Domain Methods

    #region DomainUpkBuilderBase Implementation

    public override int GetBuilderSize() {
      BuilderSize = sizeof(int)
                  + getStringSize();

      return BuilderSize;
    }

    #endregion DomainUpkBuilderBase Implementation

    #region Private Methods

    private int getStringSize() {
      const int maxAnsiCode = 255;

      return String.Any(c => c > maxAnsiCode) ? String.Length * 2 : String.Length;
    }

    #endregion Private Methods

  }

}
