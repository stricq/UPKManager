using System.Threading.Tasks;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Helpers;
using UpkManager.Domain.Models.Tables;


namespace UpkManager.Domain.Models.Objects {

  public class DomainObjectArchetypeBase : DomainObjectBase {

    #region Properties

    public override ObjectType ObjectType => ObjectType.ArchetypeObjectReference;

    public int ArchetypeObjectReference { get; set; } // This is still just a guess but it is related to the export object having an ArchetypeReference and it seems to point to a type.

    public DomainNameTableIndex ArchetypeObjectNameIndex { get; set; }

    #endregion Properties

    #region Domain Methods

    public override async Task ReadDomainObject(ByteArrayReader reader, DomainHeader header, DomainExportTableEntry export, bool skipProperties, bool skipParse) {
      ArchetypeObjectReference = reader.ReadInt32();

      ArchetypeObjectNameIndex = header.GetObjectTableEntry(ArchetypeObjectReference)?.NameIndex;

      await base.ReadDomainObject(reader, header, export, skipProperties, skipParse);
    }

    #endregion Domain Methods

  }

}
