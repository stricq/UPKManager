using System;
using System.ComponentModel.Composition;

using UpkManager.Domain.Models.Objects;


namespace UpkManager.Domain.Models.Tables {

  [Export]
  [PartCreationPolicy(CreationPolicy.NonShared)]
  public class DomainExportTableEntry : DomainObjectTableEntry {

    #region Private Fields
    //
    // Repository Fields
    //
    private int   typeReference;
    private int parentReference;

    private int archetypeReference;

    private uint flagsHigh;
    private uint flagsLow;

    private int serialDataSize;
    private int serialDataOffset;

    private uint exportFlags;

    private int netObjectCount;

    private byte[] guid;

    private uint unknown1;

    private byte[] unknown2;

    private DomainObjectBase domainObject;

    private DomainNameTableIndex typeNameIndex;
    //
    // Domain Fields
    //
    private bool isSelected;
    private bool isErrored;

    #endregion Private Fields

    #region Constructor

    public DomainExportTableEntry() {
      guid = new byte[16];
    }

    #endregion Constructor

    #region Properties

    public int TypeReference {
      get { return typeReference; }
      set { SetField(ref typeReference, value, () => TypeReference); }
    }

    public int ParentReference {
      get { return parentReference; }
      set { SetField(ref parentReference, value, () => ParentReference); }
    }

    public int ArchetypeReference {
      get { return archetypeReference; }
      set { SetField(ref archetypeReference, value, () => ArchetypeReference); }
    }

    public uint FlagsHigh {
      get { return flagsHigh; }
      set { SetField(ref flagsHigh, value, () => FlagsHigh); }
    }

    public uint FlagsLow {
      get { return flagsLow; }
      set { SetField(ref flagsLow, value, () => FlagsLow); }
    }

    public int SerialDataSize {
      get { return serialDataSize; }
      set { SetField(ref serialDataSize, value, () => SerialDataSize); }
    }

    public int SerialDataOffset {
      get { return serialDataOffset; }
      set { SetField(ref serialDataOffset, value, () => SerialDataOffset); }
    }

    public uint ExportFlags {
      get { return exportFlags; }
      set { SetField(ref exportFlags, value, () => ExportFlags); }
    }

    public int NetObjectCount {
      get { return netObjectCount; }
      set { SetField(ref netObjectCount, value, () => NetObjectCount); }
    }

    public byte[] Guid {
      get { return guid; }
      set { SetField(ref guid, value, () => Guid); }
    }

    public uint Unknown1 {
      get { return unknown1; }
      set { SetField(ref unknown1, value, () => Unknown1); }
    }

    public byte[] Unknown2 {
      get { return unknown2; }
      set { SetField(ref unknown2, value, () => Unknown2); }
    }

    public DomainObjectBase DomainObject {
      get { return domainObject; }
      set { SetField(ref domainObject, value, () => DomainObject); }
    }

    public DomainNameTableIndex TypeNameIndex {
      get { return typeNameIndex; }
      set { SetField(ref typeNameIndex, value, () => TypeNameIndex); }
    }

    #endregion Properties

    #region Domain Properties

    public bool IsSelected {
      get { return isSelected; }
      set { SetField(ref isSelected, value, () => IsSelected); }
    }

    public bool IsErrored {
      get { return isErrored; }
      set { SetField(ref isErrored, value, () => IsErrored); }
    }

    public string GuidString => new Guid(guid).ToString("B");

    public string TypeName => typeNameIndex?.Name;

    #endregion Domain Properties

  }

}
