using UpkManager.Domain.Constants;


namespace UpkManager.Domain.Models.Properties {

  public sealed class DomainPropertyInterfaceValue : DomainPropertyObjectValue {

    #region Domain Properties

    public override PropertyType PropertyType => PropertyType.InterfaceProperty;

    #endregion Domain Properties

  }

}