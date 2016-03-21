using UpkManager.Domain.Constants;


namespace UpkManager.Domain.Models.Properties {

  public sealed class DomainPropertyInterfaceValue : DomainPropertyObjectValue {

    #region Domain Properties

    public override PropertyTypes PropertyType => PropertyTypes.InterfaceProperty;

    #endregion Domain Properties

  }

}