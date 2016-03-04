using System.Threading.Tasks;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Helpers;


namespace UpkManager.Domain.Models.Properties {

  public class DomainPropertyFloatValue : DomainPropertyValueBase {

    #region Private Fields

    private float floatValue;

    #endregion Private Fields

    #region Properties

    public override PropertyType PropertyType => PropertyType.FloatProperty;

    public override object Value => floatValue;

    #endregion Properties

    #region Domain Methods

    public override async Task ReadPropertyValue(ByteArrayReader reader, int size, DomainHeader header) {
      floatValue = await Task.Run(() => reader.ReadSingle());
    }

    #endregion Domain Methods

  }

}
