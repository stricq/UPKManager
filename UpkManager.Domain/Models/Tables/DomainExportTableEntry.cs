using System;
using System.Threading.Tasks;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Helpers;
using UpkManager.Domain.Models.Objects;


namespace UpkManager.Domain.Models.Tables {

  public class DomainExportTableEntry : DomainObjectTableEntry {

    #region Constructor

    public DomainExportTableEntry() {
      NameIndex = new DomainNameTableIndex();
    }

    #endregion Constructor

    #region Properties

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

    #endregion Properties

    #region Domain Properties

    public ByteArrayReader DomainObjectReader { get; set; }

    public DomainObjectBase DomainObject { get; set; }

    public DomainNameTableIndex TypeReferenceNameIndex { get; set; }

    public DomainNameTableIndex ParentReferenceNameIndex { get; set; }

    public DomainNameTableIndex OwnerReferenceNameIndex { get; set; }

    public DomainNameTableIndex ArchetypeReferenceNameIndex { get; set; }

    public bool IsErrored { get; set; }

    #endregion Domain Properties

    #region Domain Methods

    public async Task ReadExportTableEntry(ByteArrayReader reader, DomainHeader header) {
      TypeReference   = reader.ReadInt32();
      ParentReference = reader.ReadInt32();
      OwnerReference  = reader.ReadInt32();

      NameIndex.ReadNameTableIndex(reader, header);

      ArchetypeReference = reader.ReadInt32();

      FlagsHigh = reader.ReadUInt32();
      FlagsLow  = reader.ReadUInt32();

      SerialDataSize   = reader.ReadInt32();
      SerialDataOffset = reader.ReadInt32();

      ExportFlags = reader.ReadUInt32();

      NetObjectCount = reader.ReadInt32();

      Guid = await reader.ReadBytes(16);

      Unknown1 = reader.ReadUInt32();

      Unknown2 = await reader.ReadBytes(sizeof(uint) * NetObjectCount);
    }

    public void ExpandReferences(DomainHeader header) {
           TypeReferenceNameIndex = header.GetObjectTableEntry(TypeReference)?.NameIndex;
         ParentReferenceNameIndex = header.GetObjectTableEntry(ParentReference)?.NameIndex;
          OwnerReferenceNameIndex = header.GetObjectTableEntry(OwnerReference)?.NameIndex;
      ArchetypeReferenceNameIndex = header.GetObjectTableEntry(ArchetypeReference)?.NameIndex;
    }

    public async Task ReadDomainObject(ByteArrayReader reader) {
      DomainObjectReader = await reader.Splice(SerialDataOffset, SerialDataSize);
    }

    public async Task ParseDomainObject(DomainHeader header, bool skipProperties, bool skipParse) {
      DomainObject = objectTypeFactory();

      await DomainObject.ReadDomainObject(DomainObjectReader, header, this, skipProperties, skipParse);
    }

    #endregion Domain Methods

    #region DomainUpkBuilderBase Implementation

    public override int GetBuilderSize() {
      BuilderSize = base.GetBuilderSize()
                  + sizeof(int) * 6
                  + sizeof(uint) * 4
                  + Guid.Length
                  + Unknown2.Length;

      return BuilderSize;
    }

    #endregion DomainUpkBuilderBase Implementation

    #region Private Methods

    private DomainObjectBase objectTypeFactory() {
      ObjectType type;

      Enum.TryParse(TypeReferenceNameIndex?.Name, true, out type);

      if (type == ObjectType.Unknown && TypeReferenceNameIndex != null) {
        if (TypeReferenceNameIndex.Name.StartsWith("CustomUIComp", StringComparison.CurrentCultureIgnoreCase) ||
            TypeReferenceNameIndex.Name.StartsWith("Distribution", StringComparison.CurrentCultureIgnoreCase) ||
            TypeReferenceNameIndex.Name.StartsWith("UIComp",       StringComparison.CurrentCultureIgnoreCase) ||
            TypeReferenceNameIndex.Name.EndsWith("Component",      StringComparison.CurrentCultureIgnoreCase)) type = ObjectType.ArchetypeObjectReference;
      }

      switch(type) {
        case ObjectType.ArchetypeObjectReference: return new DomainObjectArchetypeBase();
        case ObjectType.ObjectRedirector:         return new DomainObjectObjectRedirector();
        case ObjectType.Texture2D:                return new DomainObjectTexture2D();

        default: return new DomainObjectBase();
      }
    }

    #endregion Private Methods

  }

}
