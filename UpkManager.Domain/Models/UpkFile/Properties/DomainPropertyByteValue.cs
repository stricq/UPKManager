using System;
using System.Threading.Tasks;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Helpers;
using UpkManager.Domain.Models.UpkFile.Tables;


namespace UpkManager.Domain.Models.UpkFile.Properties {

  public sealed class DomainPropertyByteValue : DomainPropertyNameValue {

    #region Properties

    private byte? byteValue { get; set; }

    #endregion Properties

    #region Domain Properties

    public override PropertyTypes PropertyType => PropertyTypes.ByteProperty;

    public override object PropertyValue => byteValue ?? base.PropertyValue;

    public override string PropertyString => byteValue.HasValue ? $"{byteValue.Value}" : base.PropertyString;

    #endregion Domain Properties

    #region Domain Methods

    public override async Task ReadPropertyValue(ByteArrayReader reader, int size, DomainHeader header) {
      if (size == 8) await base.ReadPropertyValue(reader, size, header);
      else byteValue = reader.ReadByte();
    }

    public override void SetPropertyValue(object value) {
      DomainNameTableIndex index = value as DomainNameTableIndex;

      if (index == null) {
        DomainNameTableEntry entry = value as DomainNameTableEntry;

        if (entry != null) {
          index = new DomainNameTableIndex();

          index.SetNameTableIndex(entry);
        }
      }

      if (index != null) {
        NameIndexValue = index;

        return;
      }

      if (value is bool && byteValue.HasValue) byteValue = Convert.ToByte(value);
    }

    #endregion Domain Methods

    #region DomainUpkBuilderBase Implementation

    public override int GetBuilderSize() {
      BuilderSize = byteValue.HasValue ? sizeof(byte) : NameIndexValue.GetBuilderSize();

      return BuilderSize;
    }

    public override async Task WriteBuffer(ByteArrayWriter Writer, int CurrentOffset) {
      if (byteValue.HasValue) Writer.WriteByte(byteValue.Value);
      else await NameIndexValue.WriteBuffer(Writer, CurrentOffset);
    }

    #endregion DomainUpkBuilderBase Implementation

  }

}
