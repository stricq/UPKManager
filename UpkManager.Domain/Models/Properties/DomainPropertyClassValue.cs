using UpkManager.Domain.Constants;


namespace UpkManager.Domain.Models.Properties {

  public class DomainPropertyClassValue : DomainPropertyObjectValue {

    #region Overrides

    public override PropertyType PropertyType => PropertyType.ClassProperty;

    #endregion Overrides

  }

}