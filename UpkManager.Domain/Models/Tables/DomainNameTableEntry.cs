using System.Threading.Tasks;

using UpkManager.Domain.Helpers;


namespace UpkManager.Domain.Models.Tables {

  public sealed class DomainNameTableEntry : DomainUpkBuilderBase {

    #region Constructor

    public DomainNameTableEntry() {
      Name = new DomainString();
    }

    #endregion Constructor

    #region Properties

    public DomainString Name { get; }

    public ulong Flags { get; private set; }

    #endregion Properties

    #region Domain Properties

    public int TableIndex { get; set; }

    #endregion Domain Properties

    #region Domain Methods

    public async Task ReadNameTableEntry(ByteArrayReader reader) {
      await Name.ReadString(reader);

      Flags = reader.ReadUInt64();
    }

    #endregion Domain Methods

    #region DomainUpkBuilderBase Implementation

    public override int GetBuilderSize() {
      BuilderSize = Name.GetBuilderSize()
                  + sizeof(ulong);

      return BuilderSize;
    }

    public override async Task WriteBuffer(ByteArrayWriter Writer, int CurrentOffset) {
      await Name.WriteBuffer(Writer, 0);

      Writer.WriteUInt64(Flags);
    }

    #endregion DomainUpkBuilderBase Implementation

  }

}
