using System;
using System.Text;
using System.Threading.Tasks;

using UpkManager.Domain.Contracts;


namespace UpkManager.Domain.Models {

  public class DomainString {

    #region Properties

    public int Size { get; set; }

    public string String { get; set; }

    #endregion Properties

    #region Domain Methods

    public async Task ReadString(IByteArrayReader data) {
      Size = data.ReadInt32();

      if (Size == 0) {
        String = String.Empty;

        return;
      }

      if (Size < 0) {
        Size = -Size * 2;

        byte[] str = await data.ReadBytes(Size);

        String = Encoding.Unicode.GetString(str);
      }
      else {
        byte[] str = await data.ReadBytes(Size - 1);

        data.Skip(1); // NULL Terminator

        String = Encoding.ASCII.GetString(str);
      }
    }

    #endregion Domain Methods

  }

}
