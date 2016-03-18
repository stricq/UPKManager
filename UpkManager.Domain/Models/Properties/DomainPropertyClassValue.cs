using UpkManager.Domain.Constants;


namespace UpkManager.Domain.Models.Properties {

  public sealed class DomainPropertyClassValue : DomainPropertyObjectValue {

    #region Overrides

    public override PropertyType PropertyType => PropertyType.ClassProperty;

    #endregion Overrides

  }

}