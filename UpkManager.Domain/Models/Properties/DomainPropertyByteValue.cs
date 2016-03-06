using System.Threading.Tasks;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Helpers;


namespace UpkManager.Domain.Models.Properties {

  public class DomainPropertyByteValue : DomainPropertyNameValue {

    #region Properties

    public byte? ByteValue { get; set; }

    public override PropertyType PropertyType => PropertyType.ByteProperty;

    public override object PropertyValue => ByteValue ?? base.PropertyValue;

    public override string PropertyString => ByteValue.HasValue ? $"{ByteValue.Value}" : base.PropertyString;

    #endregion Properties

    #region Domain Methods

    public override async Task ReadPropertyValue(ByteArrayReader reader, int size, DomainHeader header) {
      if (size == 8) await base.ReadPropertyValue(reader, size, header);
      else ByteValue = reader.ReadByte();
    }

    #endregion Domain Methods

  }

}
