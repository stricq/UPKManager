using System.Threading.Tasks;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Helpers;


namespace UpkManager.Domain.Models.Properties {

  public class DomainPropertyBoolValue : DomainPropertyValueBase {

    #region Private Fields

    private uint boolValue;

    #endregion Private Fields

    #region Properties

    public override PropertyType PropertyType => PropertyType.BoolProperty;

    public override object Value => boolValue;

    #endregion Properties

    #region Domain Methods

    public override async Task ReadPropertyValue(ByteArrayReader reader, int size, DomainHeader header) {
      boolValue = await Task.Run(() => reader.ReadUInt32());
    }

    #endregion Domain Methods

  }

}
