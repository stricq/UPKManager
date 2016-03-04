using System.Threading.Tasks;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Helpers;
using UpkManager.Domain.Models.Tables;


namespace UpkManager.Domain.Models.Objects {

  public class DomainObjectDistributionBase : DomainObjectBase {

    #region Constructor

    public DomainObjectDistributionBase(ObjectType DistributionType) {
      ObjectType = DistributionType;
    }

    #endregion Constructor

    #region Properties

    public int Reference { get; set; }

    #endregion Properties

    #region Domain Properties

    public DomainNameTableIndex ReferenceNameIndex { get; set; }

    #endregion Domain Properties

    #region Overrides

    public override ObjectType ObjectType { get; }

    public override async Task ReadDomainObject(ByteArrayReader reader, DomainHeader header, DomainExportTableEntry export, bool skipProperties, bool skipParse) {
      Reference = reader.ReadInt32();

      ReferenceNameIndex = header.GetObjectTableEntry(Reference)?.NameIndex;

      await base.ReadDomainObject(reader, header, export, skipProperties, skipParse);
    }

    #endregion Overrides

  }

}
