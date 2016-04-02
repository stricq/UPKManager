using System.Threading.Tasks;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Helpers;
using UpkManager.Domain.Models.UpkFile.Tables;


namespace UpkManager.Domain.Models.UpkFile.Objects {

  public sealed class DomainObjectObjectRedirector : DomainObjectBase {

    #region Properties

    public int ObjectTableReference { get; private set; }

    #endregion Properties

    #region Domain Properties

    public override ObjectTypes ObjectType => ObjectTypes.ObjectRedirector;

    public DomainNameTableIndex ObjectReferenceNameIndex { get; set; }

    #endregion Domain Properties

    #region Domain Methods

    public override async Task ReadDomainObject(ByteArrayReader reader, DomainHeader header, DomainExportTableEntry export, bool skipProperties, bool skipParse) {
      await base.ReadDomainObject(reader, header, export, skipProperties, skipParse);

      if (skipParse) return;

      ObjectTableReference = reader.ReadInt32();

      ObjectReferenceNameIndex = header.GetObjectTableEntry(ObjectTableReference)?.NameTableIndex;
    }

    #endregion Domain Methods

    #region DomainUpkBuilderBase Implementation

    public override int GetBuilderSize() {
      BuilderSize = PropertyHeader.GetBuilderSize()
                  + sizeof(int);

      return BuilderSize;
    }

    public override async Task WriteBuffer(ByteArrayWriter Writer, int CurrentOffset) {
      await PropertyHeader.WriteBuffer(Writer, CurrentOffset);

      Writer.WriteInt32(ObjectTableReference);
    }

    #endregion DomainUpkBuilderBase Implementation

  }

}
