using System.Threading.Tasks;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Helpers;
using UpkManager.Domain.Models.UpkFile.Tables;


namespace UpkManager.Domain.Models.UpkFile.Objects {

  public class DomainObjectArchetypeBase : DomainObjectBase {

    #region Properties

    public int ArchetypeObjectReference { get; private set; } // This is still just a guess but it is related to the export object having an ArchetypeReference and it seems to point to a type.

    #endregion Properties

    #region Domain Properties

    public override ObjectTypes ObjectType => ObjectTypes.ArchetypeObjectReference;

    public DomainNameTableIndex ArchetypeObjectNameIndex { get; private set; }

    #endregion domain Properties

    #region Domain Methods

    public override async Task ReadDomainObject(ByteArrayReader reader, DomainHeader header, DomainExportTableEntry export, bool skipProperties, bool skipParse) {
      ArchetypeObjectReference = reader.ReadInt32();

      ArchetypeObjectNameIndex = header.GetObjectTableEntry(ArchetypeObjectReference)?.NameTableIndex;

      await base.ReadDomainObject(reader, header, export, skipProperties, skipParse);
    }

    #endregion Domain Methods

    #region DomainUpkBuilderBase Implementation

    public override int GetBuilderSize() {
      BuilderSize = sizeof(int)
                  + base.GetBuilderSize();

      return BuilderSize;
    }

    public override async Task WriteBuffer(ByteArrayWriter Writer, int CurrentOffset) {
      Writer.WriteInt32(ArchetypeObjectReference);

      await base.WriteBuffer(Writer, CurrentOffset);
    }

    #endregion DomainUpkBuilderBase Implementation

  }

}
