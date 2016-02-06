using UpkManager.Domain.Constants;


namespace UpkManager.Domain.Models.Properties {

  public class DomainPropertyStrValue : DomainPropertyValueBase {

    #region Private Fields

    private new DomainString data;

    #endregion Private Fields

    #region Overrides

    public override PropertyType PropertyType => PropertyType.StrProperty;

    public override object Value {
      get { return data; }
      set { data = (DomainString)value; }
    }

    #endregion Overrides

    #region Methods

    public override string ToString() {
      return data.String;
    }

    #endregion Methods

  }

}
