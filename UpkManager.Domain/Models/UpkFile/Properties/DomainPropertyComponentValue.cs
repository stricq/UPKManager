using UpkManager.Domain.Constants;


namespace UpkManager.Domain.Models.UpkFile.Properties {

  public sealed class DomainPropertyComponentValue : DomainPropertyObjectValue {

    #region Overrides

    public override PropertyTypes PropertyType => PropertyTypes.ComponentProperty;

    #endregion Overrides

  }

}