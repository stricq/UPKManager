using UpkManager.Domain.Constants;
using UpkManager.Domain.Models.Tables;


namespace UpkManager.Domain.Models.Properties {

  public class DomainPropertyNameValue : DomainPropertyValueBase {

    #region Private Fields

    protected new DomainNameTableIndex data;

    #endregion Private Fields

    #region Properties

    public override PropertyType PropertyType => PropertyType.NameProperty;

    public override object Value {
      get { return data; }
      set { data = (DomainNameTableIndex)value; }
    }

    #endregion Properties

    #region Methods

    public override string ToString() {
      return $"{data.Name}";
    }

    #endregion Methods

  }

}
