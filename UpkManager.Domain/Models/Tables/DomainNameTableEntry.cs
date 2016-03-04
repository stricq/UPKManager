using System.Threading.Tasks;

using UpkManager.Domain.Helpers;


namespace UpkManager.Domain.Models.Tables {

  public class DomainNameTableEntry  {

    #region Public Fields

    public int TableIndex { get; set; }

    public DomainString Name { get; set; }

    public ulong Flags { get; set; }

    #endregion Public Fields

    #region Domain Methods

    public async Task ReadNameTableEntry(ByteArrayReader reader) {
      Name = new DomainString();

      await Name.ReadString(reader);

      Flags = reader.ReadUInt64();
    }

    #endregion Domain Methods

  }

}
