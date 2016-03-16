

namespace UpkManager.Domain.Constants {

  public enum PropertyType {

    UnknownProperty,

    BoolProperty, // 0 bytes

    IntProperty, // int : 4 bytes

    FloatProperty, // float : 4 bytes

    ObjectProperty, InterfaceProperty, ComponentProperty, ClassProperty, // object reference : 4 bytes

    GuidProperty, // guid : 16 bytes

    NameProperty, ByteProperty, // DomainNameIndex : 8 bytes

    StringProperty, // uint + string : 4 + value of uint

    StructProperty, // variable : depends on structNameIndex

    ArrayProperty // uint elements, elements * element size (lookup in export or import table)

  }

}
