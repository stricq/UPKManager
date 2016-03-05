using System.Threading.Tasks;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Helpers;


namespace UpkManager.Domain.Models.Properties {

  public class DomainPropertyValueBase {

    #region Protected Fields

    protected ByteArrayReader DataReader;

    #endregion Protected Fields

    #region Properties

    public virtual PropertyType PropertyType => PropertyType.UnknownProperty;

    public virtual object PropertyValue => DataReader.GetByteArray();

    public virtual string PropertyString => $"{DataReader.GetByteArray().Length} Bytes of Data";

    #endregion Properties

    #region Domain Methods

    public virtual async Task ReadPropertyValue(ByteArrayReader reader, int size, DomainHeader header) {
      DataReader = await reader.ReadByteArray(size);
    }

    #endregion Domain Methods

  }

}
