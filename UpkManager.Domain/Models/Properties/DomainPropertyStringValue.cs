using System.Threading.Tasks;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Helpers;


namespace UpkManager.Domain.Models.Properties {

  public class DomainPropertyStringValue : DomainPropertyValueBase {

    #region Private Fields

    private readonly DomainString stringValue;

    #endregion Private Fields

    #region Constructor

    public DomainPropertyStringValue() {
      stringValue = new DomainString();
    }

    #endregion Constructor

    #region Properties

    public override PropertyType PropertyType => PropertyType.StringProperty;

    public override object PropertyValue => stringValue;

    public override string PropertyString => stringValue.String;

    #endregion Properties

    #region Domain Methods

    public override async Task ReadPropertyValue(ByteArrayReader reader, int size, DomainHeader header) {
      await stringValue.ReadString(reader);
    }

    #endregion Domain Methods

  }

}
