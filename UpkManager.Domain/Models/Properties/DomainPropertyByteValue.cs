using System.Threading.Tasks;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Helpers;


namespace UpkManager.Domain.Models.Properties {

  public sealed class DomainPropertyByteValue : DomainPropertyNameValue {

    #region Properties

    private byte? byteValue { get; set; }

    #endregion Properties

    #region Domain Properties

    public override PropertyType PropertyType => PropertyType.ByteProperty;

    public override object PropertyValue => byteValue ?? base.PropertyValue;

    public override string PropertyString => byteValue.HasValue ? $"{byteValue.Value}" : base.PropertyString;

    #endregion Domain Properties

    #region Domain Methods

    public override async Task ReadPropertyValue(ByteArrayReader reader, int size, DomainHeader header) {
      if (size == 8) await base.ReadPropertyValue(reader, size, header);
      else byteValue = reader.ReadByte();
    }

    #endregion Domain Methods

    #region DomainUpkBuilderBase Implementation

    public override int GetBuilderSize() {
      BuilderSize = byteValue.HasValue ? sizeof(byte) : NameIndexValue.GetBuilderSize();

      return BuilderSize;
    }

    public override async Task WriteBuffer(ByteArrayWriter Writer) {
      if (byteValue.HasValue) Writer.WriteByte(byteValue.Value);
      else await NameIndexValue.WriteBuffer(Writer);
    }

    #endregion DomainUpkBuilderBase Implementation

  }

}
