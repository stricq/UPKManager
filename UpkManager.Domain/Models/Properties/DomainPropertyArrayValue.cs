using UpkManager.Domain.Constants;


namespace UpkManager.Domain.Models.Properties {

  public sealed class DomainPropertyArrayValue : DomainPropertyValueBase {

    #region Properties

    public override PropertyType PropertyType => PropertyType.ArrayProperty;

    #endregion Properties

  }

}
