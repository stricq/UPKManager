using System;

using UpkManager.Domain.Constants;


namespace UpkManager.Domain.Models.UpkFile.Properties {

  public sealed class DomainPropertyGuidValue : DomainPropertyValueBase {

    #region Domain Properties

    public override PropertyTypes PropertyType => PropertyTypes.GuidProperty;

    public override string PropertyString => $"{new Guid(DataReader.GetBytes()):B}";

    #endregion Domain Properties

  }

}
