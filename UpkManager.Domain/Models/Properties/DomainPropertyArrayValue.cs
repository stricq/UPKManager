using UpkManager.Domain.Constants;


namespace UpkManager.Domain.Models.Properties {

  public class DomainPropertyArrayValue : DomainPropertyValueBase {

    #region Overrides

    public override PropertyType PropertyType => PropertyType.ArrayProperty;

    #endregion Overrides

  }

}
