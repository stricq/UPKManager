using System.Threading.Tasks;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Helpers;


namespace UpkManager.Domain.Models.UpkFile.Properties {

  public sealed class DomainPropertyFloatValue : DomainPropertyValueBase {

    #region Properties

    private float floatValue { get; set; }

    #endregion Properties

    #region Domain Properties

    public override PropertyTypes PropertyType => PropertyTypes.FloatProperty;

    public override object PropertyValue => floatValue;

    public override string PropertyString => $"{floatValue}";

    #endregion Domain Properties

    #region Domain Methods

    public override async Task ReadPropertyValue(ByteArrayReader reader, int size, DomainHeader header) {
      floatValue = await Task.Run(() => reader.ReadSingle());
    }

    #endregion Domain Methods

    #region DomainUpkBuilderBase Implementation

    public override int GetBuilderSize() {
      BuilderSize = sizeof(float);

      return BuilderSize;
    }

    public override async Task WriteBuffer(ByteArrayWriter Writer, int CurrentOffset) {
      await Task.Run(() => Writer.WriteSingle(floatValue));
    }

    #endregion DomainUpkBuilderBase Implementation

  }

}
