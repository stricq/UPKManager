using System.Threading.Tasks;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Helpers;
using UpkManager.Domain.Models.Tables;


namespace UpkManager.Domain.Models.Properties {

  public class DomainPropertyStructValue : DomainPropertyValueBase {

    #region Constructor

    public DomainPropertyStructValue() {
      StructNameIndex = new DomainNameTableIndex();
    }

    #endregion Constructor

    #region Properties

    public override PropertyType PropertyType => PropertyType.StructProperty;

    public DomainNameTableIndex StructNameIndex { get; set; }

    #endregion Properties

    #region Domain Methods

    public override async Task ReadPropertyValue(ByteArrayReader reader, int size, DomainHeader header) {
      await Task.Run(() => StructNameIndex.ReadNameTableIndex(reader, header));

      await base.ReadPropertyValue(reader, size, header);
    }

    #endregion Domain Methods

  }

}
