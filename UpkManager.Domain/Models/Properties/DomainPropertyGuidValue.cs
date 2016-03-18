using System;

using UpkManager.Domain.Constants;


namespace UpkManager.Domain.Models.Properties {

  public sealed class DomainPropertyGuidValue : DomainPropertyValueBase {

    #region Domain Properties

    public override PropertyType PropertyType => PropertyType.GuidProperty;

    public override string PropertyString => $"{new Guid(DataReader.GetBytes()):B}";

    #endregion Domain Properties

  }

}
