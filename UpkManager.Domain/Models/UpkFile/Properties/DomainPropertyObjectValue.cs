using System.Threading.Tasks;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Helpers;
using UpkManager.Domain.Models.UpkFile.Tables;


namespace UpkManager.Domain.Models.UpkFile.Properties {

  public class DomainPropertyObjectValue : DomainPropertyIntValue {

    #region Properties

    public DomainNameTableIndex ObjectIndexName { get; private set; }

    public override PropertyTypes PropertyType => PropertyTypes.ObjectProperty;

    public override string PropertyString => ObjectIndexName.Name;

    #endregion Properties

    #region Domain Methods

    public override async Task ReadPropertyValue(ByteArrayReader reader, int size, DomainHeader header) {
      await base.ReadPropertyValue(reader, size, header);

      ObjectIndexName = header.GetObjectTableEntry(IntValue)?.NameTableIndex;
    }

    #endregion Domain Methods

  }

}
