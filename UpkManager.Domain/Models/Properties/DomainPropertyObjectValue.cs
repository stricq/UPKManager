using System.Threading.Tasks;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Helpers;
using UpkManager.Domain.Models.Tables;


namespace UpkManager.Domain.Models.Properties {

  public class DomainPropertyObjectValue : DomainPropertyIntValue {

    #region Properties

    public DomainNameTableIndex ObjectIndexName { get; set; }

    public override PropertyType PropertyType => PropertyType.ObjectProperty;

    public override string PropertyString => ObjectIndexName.Name;

    #endregion Properties

    #region Domain Methods

    public override async Task ReadPropertyValue(ByteArrayReader reader, int size, DomainHeader header) {
      await base.ReadPropertyValue(reader, size, header);

      ObjectIndexName = header.GetObjectTableEntry(IntValue)?.NameIndex;
    }

    #endregion Domain Methods

  }

}
