using System;

using UpkManager.Domain.Constants;


namespace UpkManager.Domain.Models.Properties {

  public class DomainPropertyGuidValue : DomainPropertyValueBase {

    #region Overrides

    public override PropertyType PropertyType => PropertyType.GuidProperty;

    public override string ToString() {
      Guid guid = new Guid(data);

      return $"{guid.ToString("B")}";
    }

    #endregion Overrides

  }

}
