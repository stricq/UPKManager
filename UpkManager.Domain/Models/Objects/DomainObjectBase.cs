using System.Threading.Tasks;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Helpers;
using UpkManager.Domain.Models.Properties;
using UpkManager.Domain.Models.Tables;


namespace UpkManager.Domain.Models.Objects {

  public class DomainObjectBase {

    #region Constructor

    public DomainObjectBase() {
      PropertyHeader = new DomainPropertyHeader();
    }

    #endregion Constructor

    #region Properties

    public DomainPropertyHeader PropertyHeader { get; set; }

    public int AdditionalDataOffset { get; set; }

    public ByteArrayReader AdditionalDataReader { get; set; }

    #endregion Properties

    #region Domain Properties

    public virtual bool IsExportable => false;

    public virtual bool IsViewable => false;

    public virtual ObjectType ObjectType => ObjectType.Unknown;

    #endregion Domain Properties

    #region Domain Methods

    public virtual async Task ReadDomainObject(ByteArrayReader reader, DomainHeader header, DomainExportTableEntry export, bool skipProperties, bool skipParse) {
      if (!skipProperties) await PropertyHeader.ReadPropertyHeader(reader, header);

      if (reader.Remaining == 0) return;

      AdditionalDataOffset = export.SerialDataOffset + reader.CurrentOffset;

      AdditionalDataReader = await reader.Splice();
    }

    #endregion Domain Methods

  }

}
