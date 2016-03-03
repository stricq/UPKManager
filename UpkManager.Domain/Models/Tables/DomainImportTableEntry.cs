using System.Threading.Tasks;

using UpkManager.Domain.Contracts;


namespace UpkManager.Domain.Models.Tables {

  public class DomainImportTableEntry : DomainObjectTableEntry {

    #region Constructor

    public DomainImportTableEntry() {
      PackageNameIndex = new DomainNameTableIndex();
      TypeNameIndex    = new DomainNameTableIndex();
      NameIndex        = new DomainNameTableIndex();
    }

    #endregion Constructor

    #region Properties

    public DomainNameTableIndex PackageNameIndex { get; set; }

    public DomainNameTableIndex TypeNameIndex { get; set; }
    //
    // OwnerReference in ObjectTableEntry
    //
    // NameTableIndex in ObjectTableEntry
    //
    #endregion Properties

    #region Domain Properties

    public DomainNameTableIndex OwnerReferenceNameIndex { get; set; }

    #endregion Domain Properties

    #region Domain Methods

    public async Task ReadImportTableEntry(IByteArrayReader reader, DomainHeader header) {
      await Task.Run(() => PackageNameIndex.ReadNameTableIndex(reader, header));

      await Task.Run(() => TypeNameIndex.ReadNameTableIndex(reader, header));

      OwnerReference = reader.ReadInt32();

      await Task.Run(() => NameIndex.ReadNameTableIndex(reader, header));
    }

    public void ExpandReferences(DomainHeader header) {
      OwnerReferenceNameIndex = header.GetObjectTableEntry(OwnerReference)?.NameIndex;
    }

    #endregion Domain Methods

  }

}
