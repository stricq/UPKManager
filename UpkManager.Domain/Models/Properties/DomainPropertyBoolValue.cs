using System.Threading.Tasks;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Helpers;


namespace UpkManager.Domain.Models.Properties {

  public sealed class DomainPropertyBoolValue : DomainPropertyValueBase {

    #region Properties

    private uint boolValue { get; set; }

    #endregion Properties

    #region Domain Properties

    public override PropertyTypes PropertyType => PropertyTypes.BoolProperty;

    public override object PropertyValue => boolValue;

    public override string PropertyString => $"{boolValue != 0}";

    #endregion Domain Properties

    #region Domain Methods

    public override async Task ReadPropertyValue(ByteArrayReader reader, int size, DomainHeader header) {
      boolValue = await Task.Run(() => reader.ReadUInt32());
    }

    #endregion Domain Methods

    #region DomainUpkBuilderBase Implementation

    public override int GetBuilderSize() {
      BuilderSize = sizeof(uint);

      return BuilderSize;
    }

    public override async Task WriteBuffer(ByteArrayWriter Writer, int CurrentOffset) {
      await Task.Run(() => Writer.WriteUInt32(boolValue));
    }

    #endregion DomainUpkBuilderBase Implementation

  }

}
