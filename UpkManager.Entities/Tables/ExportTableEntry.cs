using System;
using System.Collections.Generic;

using UpkManager.Entities.Constants;
using UpkManager.Entities.ObjectTypes;


namespace UpkManager.Entities.Tables {

  public class ExportTableEntry : ObjectTableEntry {

    #region Properties
    //
    // Parsed Properties
    //
    public int TypeReference { get; set; }

    public int ParentReference { get; set; }
    //
    // OwnerReference in UpkObject
    //
    // NameTableIndex in UpkObject
    //
    public int ArchetypeReference { get; set; }

    public uint FlagsHigh { get; set; }

    public uint FlagsLow { get; set; }

    public int SerialDataSize { get; set; }

    public int SerialDataOffset { get; set; }

    public uint ExportFlags { get; set; }

    public int NetObjectCount { get; set; }

    public byte[] Guid { get; set; }

    public uint Unknown1 { get; set; }

    public byte[] Unknown2 { get; set; } // 4 * NetObjectCount bytes
    //
    // Additional Properties
    //
    public ObjectBase UpkObject { get; set; }

    public NameTableIndex TypeNameIndex { get; set; }

    #endregion Properties

    #region Public Methods

    public void ReadExportTableEntry(byte[] data, ref int index, List<NameTableEntry> nameTable) {
      TypeReference   = BitConverter.ToInt32(data, index); index += sizeof(int);
      ParentReference = BitConverter.ToInt32(data, index); index += sizeof(int);
      OwnerReference  = BitConverter.ToInt32(data, index); index += sizeof(int);

      NameIndex = new NameTableIndex();

      NameIndex.ReadNameTableIndex(data, ref index, nameTable);

      OwnerReference = BitConverter.ToInt32(data, index); index += sizeof(int);

      FlagsHigh = BitConverter.ToUInt32(data, index); index += sizeof(uint);
      FlagsLow  = BitConverter.ToUInt32(data, index); index += sizeof(uint);

      SerialDataSize   = BitConverter.ToInt32(data, index); index += sizeof(int);
      SerialDataOffset = BitConverter.ToInt32(data, index); index += sizeof(int);

      ExportFlags = BitConverter.ToUInt32(data, index); index += sizeof(uint);

      NetObjectCount = BitConverter.ToInt32(data, index); index += sizeof(int);

      Guid = new byte[16];

      Array.ConstrainedCopy(data, index, Guid, 0, 16); index += 16;

      Unknown1 = BitConverter.ToUInt32(data, index); index += sizeof(uint);

      int size = sizeof(uint) * NetObjectCount;

      Unknown2 = new byte[size];

      Array.ConstrainedCopy(data, index, Unknown2, 0, size); index += size;
    }

    public void ReadObjectType(byte[] data, UpkHeader header, bool skipProperties, bool skipParse) {
      TypeNameIndex = header.GetObjectTableEntry(TypeReference)?.NameIndex;

      UpkObject = objectTypeFactory();

      int index = SerialDataOffset;

      UpkObject.ReadUpkObject(data, ref index, SerialDataOffset + SerialDataSize, skipProperties, skipParse, header.NameTable);
    }

    #endregion Public Methods

    #region Private Methods

    private ObjectBase objectTypeFactory() {
      ObjectType type;

      Enum.TryParse(TypeNameIndex?.Name, out type);

      switch(type) {
        case ObjectType.DistributionFloatUniform: return new ObjectDistributionFloatUniform();
        case ObjectType.ObjectRedirector:         return new ObjectObjectRedirector();
        case ObjectType.Texture2D:                return new ObjectTexture2D();

        default: return new ObjectBase();
      }
    }

    #endregion Private Methods

  }

}
