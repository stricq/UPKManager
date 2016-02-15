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

    public bool IsErrored { get; set; }

    public string ParseExceptionMessage { get; set; }

    #endregion Properties

    #region Public Methods

    public int ReadExportTableEntry(byte[] data, int index, List<NameTableEntry> nameTable) {
      string message;

      TypeReference   = BitConverter.ToInt32(data, index); index += sizeof(int);
      ParentReference = BitConverter.ToInt32(data, index); index += sizeof(int);
      OwnerReference  = BitConverter.ToInt32(data, index); index += sizeof(int);

      NameIndex = new NameTableIndex();

      NameIndex.ReadNameTableIndex(data, ref index, nameTable, out message);

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

      return index;
    }

    public void ReadObjectType(byte[] data, UpkHeader header, bool skipProperties, bool skipParse, out string message) {
      TypeNameIndex = header.GetObjectTableEntry(TypeReference)?.NameIndex;

      UpkObject = objectTypeFactory();

      int index = SerialDataOffset;

      UpkObject.ReadUpkObject(data, ref index, SerialDataOffset + SerialDataSize, skipProperties, skipParse, header, out message);
    }

    #endregion Public Methods

    #region Private Methods

    private ObjectBase objectTypeFactory() {
      ObjectType type;

      Enum.TryParse(TypeNameIndex?.Name, out type);

      switch(type) {
        case ObjectType.DistributionFloatConstant:
        case ObjectType.DistributionFloatConstantCurve:
        case ObjectType.DistributionFloatParticleParameter:
        case ObjectType.DistributionFloatUniform:
        case ObjectType.DistributionFloatUniformCurve:
        case ObjectType.DistributionVectorConstant:
        case ObjectType.DistributionVectorConstantCurve:
        case ObjectType.DistributionVectorUniform:
        case ObjectType.DistributionVectorUniformCurve: return new ObjectDistributionBase(type);
        case ObjectType.ObjectRedirector:               return new ObjectObjectRedirector();
        case ObjectType.Texture2D:                      return new ObjectTexture2D();

        default: return new ObjectBase();
      }
    }

    #endregion Private Methods

  }

}
