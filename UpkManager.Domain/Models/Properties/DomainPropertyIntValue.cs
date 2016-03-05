using System.Threading.Tasks;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Helpers;


namespace UpkManager.Domain.Models.Properties {

  public class DomainPropertyIntValue : DomainPropertyValueBase {

    #region Protected Fields

    protected int IntValue;

    #endregion Protected Fields

    #region Properties

    public override PropertyType PropertyType => PropertyType.IntProperty;

    public override object PropertyValue => IntValue;

    public override string PropertyString => $"{IntValue:N0}";

    #endregion Properties

    #region Domain Methods

    public override async Task ReadPropertyValue(ByteArrayReader reader, int size, DomainHeader header) {
      IntValue = await Task.Run(() => reader.ReadInt32());
    }

    #endregion Domain Methods

  }

}
