using System.Threading.Tasks;

using UpkManager.Domain.Contracts;
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

    public IByteArrayReader ObjectByteArrayReader { get; set; }

    public DomainObjectBase DomainObject { get; set; }

    public DomainNameTableIndex TypeReferenceNameIndex { get; set; }

    public DomainNameTableIndex ParentReferenceNameIndex { get; set; }

    public DomainNameTableIndex OwnerReferenceNameIndex { get; set; }

    public DomainNameTableIndex ArchetypeReferenceNameIndex { get; set; }

    public bool IsErrored { get; set; }

    #endregion Domain Properties

    #region Domain Methods

    public async Task ReadExportTableEntry(IByteArrayReader reader, DomainHeader header) {
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

    public async Task ReadRawObject(IByteArrayReader reader) {
      ObjectByteArrayReader = await reader.Splice(SerialDataOffset, SerialDataSize);
    }

    #endregion Domain Methods

  }

}
