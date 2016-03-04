using UpkManager.Domain.Constants;


namespace UpkManager.Domain.Models.Properties {

  public class DomainPropertyObjectValue : DomainPropertyIntValue {

    #region Overrides

    public override PropertyType PropertyType => PropertyType.ObjectProperty;

    #endregion Overrides

  }

}
