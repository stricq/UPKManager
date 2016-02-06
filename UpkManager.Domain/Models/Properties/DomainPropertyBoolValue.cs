using UpkManager.Domain.Constants;


namespace UpkManager.Domain.Models.Properties {

  public class DomainPropertyBoolValue : DomainPropertyValueBase {

    #region Private Fields

    private new uint data;

    #endregion Private Fields

    #region Properties

    public override PropertyType PropertyType => PropertyType.BoolProperty;

    public override object Value {
      get { return data; }
      set { data = (uint)value; }
    }

    #endregion Properties

    #region Methods

    public override string ToString() {
      return $"{data != 0}";
    }

    #endregion Methods

  }

}
