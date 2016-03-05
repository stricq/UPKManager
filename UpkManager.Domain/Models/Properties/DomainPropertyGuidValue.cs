using System;

using UpkManager.Domain.Constants;


namespace UpkManager.Domain.Models.Properties {

  public class DomainPropertyGuidValue : DomainPropertyValueBase {

    #region Overrides

    public override PropertyType PropertyType => PropertyType.GuidProperty;

    public override string PropertyString => $"{new Guid(DataReader.GetByteArray()):B}";

    #endregion Overrides

  }

}
