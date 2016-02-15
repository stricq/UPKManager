using System;

using UpkManager.Entities.Constants;
using UpkManager.Entities.Tables;


namespace UpkManager.Entities.PropertyTypes {

  public class Property {

    #region Properties

    public NameTableIndex NameIndex { get; set; }

    public NameTableIndex TypeNameIndex { get; set; }

    public int Size { get; set; }

    public int ArrayIndex { get; set; }

    public PropertyValueBase Value { get; set; }

    #endregion Properties

    #region Overrides

    public override string ToString() {
      return TypeNameIndex != null ? $"{TypeNameIndex.Name}.{NameIndex.Name}" : $"{NameIndex.Name}";
    }

    #endregion Overrides

    #region Public Methods

    public void ReadProperty(byte[] data, ref int index, UpkHeader header, out string message) {
      NameIndex = new NameTableIndex();

      if (!NameIndex.ReadNameTableIndex(data, ref index, header.NameTable, out message)) return;

      if (NameIndex.Name == ObjectType.None.ToString()) return;

      TypeNameIndex = new NameTableIndex();

      if (!TypeNameIndex.ReadNameTableIndex(data, ref index, header.NameTable, out message)) return;

      Size       = BitConverter.ToInt32(data, index); index += sizeof(int);
      ArrayIndex = BitConverter.ToInt32(data, index); index += sizeof(int);

      Value = propertyValueFactory();

      Value.ReadPropertyValue(data, ref index, header, out message);
    }

    #endregion Public Methods

    #region Private Methods

    private PropertyValueBase propertyValueFactory() {
      PropertyType type;

      Enum.TryParse(TypeNameIndex.Name, true, out type);

      switch(type) {
        case PropertyType.BoolProperty:      return new PropertyBoolValue      { Size = Size };
        case PropertyType.IntProperty:       return new PropertyIntValue       { Size = Size };
        case PropertyType.FloatProperty:     return new PropertyFloatValue     { Size = Size };
        case PropertyType.ObjectProperty:    return new PropertyObjectValue    { Size = Size };
        case PropertyType.InterfaceProperty: return new PropertyInterfaceValue { Size = Size };
        case PropertyType.ComponentProperty: return new PropertyComponentValue { Size = Size };
        case PropertyType.ClassProperty:     return new PropertyClassValue     { Size = Size };
        case PropertyType.GuidProperty:      return new PropertyGuidValue      { Size = Size };
        case PropertyType.NameProperty:      return new PropertyNameValue      { Size = Size };
        case PropertyType.ByteProperty:      return new PropertyByteValue      { Size = Size };
        case PropertyType.StrProperty:       return new PropertyStrValue       { Size = Size };
        case PropertyType.StructProperty:    return new PropertyStructValue    { Size = Size };
        case PropertyType.ArrayProperty:     return new PropertyArrayValue     { Size = Size };

        default: return new PropertyValueBase { Size = Size };
      }
    }

    #endregion Private Methods

  }

}
