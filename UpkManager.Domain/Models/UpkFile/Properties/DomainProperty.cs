using System;
using System.Threading.Tasks;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Helpers;
using UpkManager.Domain.Models.UpkFile.Tables;


namespace UpkManager.Domain.Models.UpkFile.Properties {

  public sealed class DomainProperty : DomainUpkBuilderBase {

    #region Constructor

    public DomainProperty() {
      NameIndex = new DomainNameTableIndex();

      TypeNameIndex = new DomainNameTableIndex();
    }

    #endregion Constructor

    #region Properties

    public DomainNameTableIndex NameIndex { get; }

    public DomainNameTableIndex TypeNameIndex { get; }

    public int Size { get; private set; }

    public int ArrayIndex { get; private set; }

    public DomainPropertyValueBase Value { get; private set; }

    #endregion Properties

    #region Domain Methods

    public async Task ReadProperty(ByteArrayReader reader, DomainHeader header) {
      await Task.Run(() => NameIndex.ReadNameTableIndex(reader, header));

      if (NameIndex.Name == ObjectTypes.None.ToString()) return;

      await Task.Run(() => TypeNameIndex.ReadNameTableIndex(reader, header));

      Size       = reader.ReadInt32();
      ArrayIndex = reader.ReadInt32();

      Value = propertyValueFactory();

      await Value.ReadPropertyValue(reader, Size, header);
    }

    #endregion Domain Methods

    #region DomainUpkBuilderBase Implementation

    public override int GetBuilderSize() {
      BuilderSize = NameIndex.GetBuilderSize();

      if (NameIndex.Name == ObjectTypes.None.ToString()) return BuilderSize;

      BuilderSize += TypeNameIndex.GetBuilderSize()
                  +  sizeof(int) * 2;

      Size = Value.GetBuilderSize();

      return BuilderSize + Size;
    }

    public override async Task WriteBuffer(ByteArrayWriter Writer, int CurrentOffset) {
      await NameIndex.WriteBuffer(Writer, 0);

      if (NameIndex.Name == ObjectTypes.None.ToString()) return;

      await TypeNameIndex.WriteBuffer(Writer, 0);

      Writer.WriteInt32(Size);

      Writer.WriteInt32(ArrayIndex);

      await Value.WriteBuffer(Writer, CurrentOffset);
    }

    #endregion DomainUpkBuilderBase Implementation

    #region Private Methods

    private DomainPropertyValueBase propertyValueFactory() {
      PropertyTypes type;

      Enum.TryParse(TypeNameIndex?.Name, true, out type);

      switch(type) {
        case PropertyTypes.BoolProperty:      return new DomainPropertyBoolValue();
        case PropertyTypes.IntProperty:       return new DomainPropertyIntValue();
        case PropertyTypes.FloatProperty:     return new DomainPropertyFloatValue();
        case PropertyTypes.ObjectProperty:    return new DomainPropertyObjectValue();
        case PropertyTypes.InterfaceProperty: return new DomainPropertyInterfaceValue();
        case PropertyTypes.ComponentProperty: return new DomainPropertyComponentValue();
        case PropertyTypes.ClassProperty:     return new DomainPropertyClassValue();
        case PropertyTypes.GuidProperty:      return new DomainPropertyGuidValue();
        case PropertyTypes.NameProperty:      return new DomainPropertyNameValue();
        case PropertyTypes.ByteProperty:      return new DomainPropertyByteValue();
        case PropertyTypes.StrProperty:       return new DomainPropertyStringValue();
        case PropertyTypes.StructProperty:    return new DomainPropertyStructValue();
        case PropertyTypes.ArrayProperty:     return new DomainPropertyArrayValue();

        default: return new DomainPropertyValueBase();
      }
    }

    #endregion Private Methods

  }

}
