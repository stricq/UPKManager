using UpkManager.Domain.Constants;


namespace UpkManager.Domain.Models.Properties {

  public class DomainPropertyComponentValue : DomainPropertyObjectValue {

    #region Overrides

    public override PropertyType PropertyType => PropertyType.ComponentProperty;

    #endregion Overrides

  }

}