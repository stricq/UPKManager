using UpkManager.Domain.Constants;


namespace UpkManager.Domain.Models.Properties {

  public class DomainPropertyGuidValue : DomainPropertyValueBase {

    #region Overrides

    public override PropertyType PropertyType => PropertyType.GuidProperty;

    #endregion Overrides

  }

}
