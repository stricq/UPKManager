using UpkManager.Domain.Constants;


namespace UpkManager.Domain.Models.Properties {

  public sealed class DomainPropertyClassValue : DomainPropertyObjectValue {

    #region Overrides

    public override PropertyTypes PropertyType => PropertyTypes.ClassProperty;

    #endregion Overrides

  }

}