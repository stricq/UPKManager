using UpkManager.Domain.Constants;


namespace UpkManager.Domain.Models.Properties {

  public class DomainPropertyObjectValue : DomainPropertyIntValue {

    #region Overrides

    public override PropertyType PropertyType => PropertyType.ObjectProperty;

    #endregion Overrides

    #region Methods

    public override string ToString() {
      return $"0x{data:X8}";
    }

    #endregion Methods

  }

}
