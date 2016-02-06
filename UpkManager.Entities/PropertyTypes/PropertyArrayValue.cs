using UpkManager.Entities.Constants;


namespace UpkManager.Entities.PropertyTypes {

  public class PropertyArrayValue : PropertyValueBase {

    #region Overrides

    public override PropertyType PropertyType => PropertyType.ArrayProperty;

    #endregion Overrides

  }

}