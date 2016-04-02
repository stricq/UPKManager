using System.Threading.Tasks;

using UpkManager.Domain.Helpers;


namespace UpkManager.Domain.Models.UpkFile.Tables {

  public sealed class DomainImportTableEntry : DomainObjectTableEntryBase {

    #region Constructor

    public DomainImportTableEntry() {
      PackageNameIndex = new DomainNameTableIndex();
      TypeNameIndex    = new DomainNameTableIndex();
      NameTableIndex        = new DomainNameTableIndex();
    }

    #endregion Constructor

    #region Properties

    public DomainNameTableIndex PackageNameIndex { get; }

    public DomainNameTableIndex TypeNameIndex { get; }
    //
    // OwnerReference in ObjectTableEntryBase
    //
    // NameTableIndex in ObjectTableEntryBase
    //
    #endregion Properties

    #region Domain Properties

    public DomainNameTableIndex OwnerReferenceNameIndex { get; set; }

    #endregion Domain Properties

    #region Domain Methods

    public async Task ReadImportTableEntry(ByteArrayReader reader, DomainHeader header) {
      await Task.Run(() => PackageNameIndex.ReadNameTableIndex(reader, header));

      await Task.Run(() => TypeNameIndex.ReadNameTableIndex(reader, header));

      OwnerReference = reader.ReadInt32();

      await Task.Run(() => NameTableIndex.ReadNameTableIndex(reader, header));
    }

    public void ExpandReferences(DomainHeader header) {
      OwnerReferenceNameIndex = header.GetObjectTableEntry(OwnerReference)?.NameTableIndex;
    }

    #endregion Domain Methods

    #region DomainUpkBuilderBase Implementation

    public override int GetBuilderSize() {
      BuilderSize = PackageNameIndex.GetBuilderSize()
                  + TypeNameIndex.GetBuilderSize()
                  + sizeof(int)
                  + NameTableIndex.GetBuilderSize();

      return BuilderSize;
    }

    public override async Task WriteBuffer(ByteArrayWriter Writer, int CurrentOffset) {
      await PackageNameIndex.WriteBuffer(Writer, 0);

      await TypeNameIndex.WriteBuffer(Writer, 0);

      Writer.WriteInt32(OwnerReference);

      await NameTableIndex.WriteBuffer(Writer, 0);
    }

    #endregion DomainUpkBuilderBase Implementation

  }

}
