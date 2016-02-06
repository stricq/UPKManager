using UpkManager.Domain.Constants;
using UpkManager.Domain.Models.Tables;


namespace UpkManager.Domain.Models.Properties {

  public class DomainPropertyStructValue : DomainPropertyValueBase {

    #region Properties

    public override PropertyType PropertyType => PropertyType.StructProperty;

    public DomainNameTableIndex StructNameIndex { get; set; }

    #endregion Properties

    #region Methods

    public override string ToString() {
      return $"{StructNameIndex.Name}";
    }

    #endregion Methods

  }

}
