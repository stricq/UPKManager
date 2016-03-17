using System.Threading.Tasks;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Helpers;
using UpkManager.Domain.Models.Tables;


namespace UpkManager.Domain.Models.Objects {

  public class DomainObjectObjectRedirector : DomainObjectBase {

    #region Properties

    public int ObjectTableReference { get; set; }

    #endregion Properties

    #region Domain Properties

    public DomainNameTableIndex ObjectReferenceNameIndex { get; set; }

    #endregion Domain Properties

    #region Overrides

    public override ObjectType ObjectType => ObjectType.ObjectRedirector;

    public override async Task ReadDomainObject(ByteArrayReader reader, DomainHeader header, DomainExportTableEntry export, bool skipProperties, bool skipParse) {
      await base.ReadDomainObject(reader, header, export, skipProperties, skipParse);

      if (skipParse) return;

      ObjectTableReference = reader.ReadInt32();

      ObjectReferenceNameIndex = header.GetObjectTableEntry(ObjectTableReference)?.NameTableIndex;
    }

    #endregion Overrides

  }

}
