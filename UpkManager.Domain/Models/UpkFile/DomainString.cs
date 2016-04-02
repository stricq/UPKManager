using System;
using System.Text;
using System.Threading.Tasks;

using UpkManager.Domain.Helpers;


namespace UpkManager.Domain.Models.UpkFile {

  public class DomainString : DomainUpkBuilderBase {

    #region Properties

    public int Size { get; private set;}

    public string String { get; private set; }

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
                  + (Size < 0 ? String.Length * 2 : String.Length + 1);

      return BuilderSize;
    }

    public override async Task WriteBuffer(ByteArrayWriter Writer, int CurrentOffset) {
      Writer.WriteInt32(Size);

      if (Size < 0) await Writer.WriteBytes(Encoding.Unicode.GetBytes(String));
      else {
        await Writer.WriteBytes(Encoding.ASCII.GetBytes(String));

        Writer.WriteByte(0);
      }
    }

    #endregion DomainUpkBuilderBase Implementation

    #region Private Methods

    //private bool isUnicode() {
    //  const int maxAnsiCode = 255;

    //  return String.Any(c => c > maxAnsiCode);
    //}

    #endregion Private Methods

  }

}
