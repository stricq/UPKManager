using UpkManager.Domain.Constants;


namespace UpkManager.Domain.Models.Properties {

  public class DomainPropertyIntValue : DomainPropertyValueBase {

    #region Private Fields

    protected new int data;

    #endregion Private Fields

    #region Overrides

    public override PropertyType PropertyType => PropertyType.IntProperty;

    public override object Value {
      get { return data; }
      set { data = (int)value; }
    }

    #endregion Overrides

    #region Methods

    public override string ToString() {
      return $"{data}";
    }

    #endregion Methods

  }

}
