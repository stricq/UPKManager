using UpkManager.Domain.Constants;


namespace UpkManager.Domain.Models.Properties {

  public class DomainPropertyFloatValue : DomainPropertyValueBase {

    #region Private Fields

    private new float data;

    #endregion Private Fields

    #region Properties

    public override PropertyType PropertyType => PropertyType.FloatProperty;

    public override object Value {
      get { return data; }
      set { data = (float)value; }
    }

    #endregion Properties

    #region Methods

    public override string ToString() {
      return $"{data}";
    }

    #endregion Methods

  }

}
