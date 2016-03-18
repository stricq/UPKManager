using System.Threading.Tasks;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Helpers;


namespace UpkManager.Domain.Models.Properties {

  public class DomainPropertyValueBase : DomainUpkBuilderBase {

    #region Properties

    protected ByteArrayReader DataReader { get; set; }

    #endregion Properties

    #region Domain Properties

    public virtual PropertyType PropertyType => PropertyType.UnknownProperty;

    public virtual object PropertyValue => DataReader.GetBytes();

    public virtual string PropertyString => $"{DataReader.GetBytes().Length:N0} Bytes of Data";

    #endregion Domain Properties

    #region Domain Methods

    public virtual async Task ReadPropertyValue(ByteArrayReader reader, int size, DomainHeader header) {
      DataReader = await reader.ReadByteArray(size);
    }

    #endregion Domain Methods

    #region DomainUpkBuilderBase Implementation

    public override int GetBuilderSize() {
      BuilderSize = DataReader?.GetBytes().Length ?? 0;

      return BuilderSize;
    }

    public override async Task WriteBuffer(ByteArrayWriter Writer) {
      await Writer.WriteBytes(DataReader?.GetBytes());
    }

    #endregion DomainUpkBuilderBase Implementation

  }

}
