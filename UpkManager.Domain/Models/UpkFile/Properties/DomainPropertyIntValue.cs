using System.Threading.Tasks;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Helpers;


namespace UpkManager.Domain.Models.UpkFile.Properties {

  public class DomainPropertyIntValue : DomainPropertyValueBase {

    #region Properties

    protected int IntValue { get; set; }

    #endregion Properties

    #region Domain Properties

    public override PropertyTypes PropertyType => PropertyTypes.IntProperty;

    public override object PropertyValue => IntValue;

    public override string PropertyString => $"{IntValue:N0}";

    #endregion Domain Properties

    #region Domain Methods

    public override async Task ReadPropertyValue(ByteArrayReader reader, int size, DomainHeader header) {
      IntValue = await Task.Run(() => reader.ReadInt32());
    }

    public override void SetPropertyValue(object value) {
      if (!(value is int)) return;

      IntValue = (int)value;
    }

    #endregion Domain Methods

    #region DomainUpkBuilderBase Implementation

    public override int GetBuilderSize() {
      BuilderSize = sizeof(int);

      return BuilderSize;
    }

    public override async Task WriteBuffer(ByteArrayWriter Writer, int CurrentOffset) {
      await Task.Run(() => Writer.WriteInt32(IntValue));
    }

    #endregion DomainUpkBuilderBase Implementation

  }

}
