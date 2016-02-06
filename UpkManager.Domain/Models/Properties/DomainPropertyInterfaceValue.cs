using UpkManager.Domain.Constants;


namespace UpkManager.Domain.Models.Properties {

  public class DomainPropertyInterfaceValue : DomainPropertyObjectValue {

    #region Overrides

    public override PropertyType PropertyType => PropertyType.InterfaceProperty;

    #endregion Overrides

  }

}