using UpkManager.Entities.Constants;


namespace UpkManager.Entities.PropertyTypes {

  public class PropertyObjectValue : PropertyIntValue {

    #region Overrides

    public override PropertyType PropertyType => PropertyType.ObjectProperty;

    public override string ToString() {
      return $"0x{data:X8}";
    }

    #endregion Overrides

  }

}
