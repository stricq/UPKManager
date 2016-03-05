using System.Threading.Tasks;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Helpers;
using UpkManager.Domain.Models.Tables;


namespace UpkManager.Domain.Models.Properties {

  public class DomainPropertyNameValue : DomainPropertyValueBase {

    #region Protected Fields

    protected DomainNameTableIndex NameIndexValue;

    #endregion Protected Fields

    #region Constructor

    public DomainPropertyNameValue() {
      NameIndexValue = new DomainNameTableIndex();
    }

    #endregion Constructor

    #region Properties

    public override PropertyType PropertyType => PropertyType.NameProperty;

    public override object PropertyValue => NameIndexValue;

    public override string PropertyString => NameIndexValue.Name;

    #endregion Properties

    #region Domain Methods

    public override async Task ReadPropertyValue(ByteArrayReader reader, int size, DomainHeader header) {
      await Task.Run(() => NameIndexValue.ReadNameTableIndex(reader, header));
    }

    #endregion Domain Methods

  }

}
