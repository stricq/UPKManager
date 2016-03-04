using System;

using STR.MvvmCommon;


namespace UpkManager.Wpf.ViewEntities.Tables {

  public class ExportTableEntryViewEntity : ObservableObject {

    #region Private Fields

    private int tableIndex;

    private int   typeReference;
    private int parentReference;
    private int  ownerReference;

    private string archetypeName;
    private string      typeName;
    private string     ownerName;
    private string          name;

    private int archetypeReference;

    private uint flagsHigh;
    private uint flagsLow;

    private int serialDataSize;
    private int serialDataOffset;

    private uint exportFlags;

    private int netObjectCount;

    private Guid guid;

    private uint unknown1;

    private bool isSelected;
    private bool isErrored;

    #endregion Private Fields

    #region Properties

    public int TableIndex {
      get { return tableIndex; }
      set { SetField(ref tableIndex, value, () => TableIndex); }
    }

    public int TypeReference {
      get { return typeReference; }
      set { SetField(ref typeReference, value, () => TypeReference); }
    }

    public int ParentReference {
      get { return parentReference; }
      set { SetField(ref parentReference, value, () => ParentReference); }
    }

    public int OwnerReference {
      get { return ownerReference; }
      set { SetField(ref ownerReference, value, () => OwnerReference); }
    }

    public string ArchetypeName {
      get { return archetypeName; }
      set { SetField(ref archetypeName, value, () => ArchetypeName); }
    }

    public string TypeName {
      get { return typeName; }
      set { SetField(ref typeName, value, () => TypeName); }
    }

    public string OwnerName {
      get { return ownerName; }
      set { SetField(ref ownerName, value, () => OwnerName); }
    }

    public string Name {
      get { return name; }
      set { SetField(ref name, value, () => Name); }
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

    public Guid Guid {
      get { return guid; }
      set { SetField(ref guid, value, () => Guid); }
    }

    public uint Unknown1 {
      get { return unknown1; }
      set { SetField(ref unknown1, value, () => Unknown1); }
    }

    public bool IsSelected {
      get { return isSelected; }
      set { SetField(ref isSelected, value, () => IsSelected); }
    }

    public bool IsErrored {
      get { return isErrored; }
      set { SetField(ref isErrored, value, () => IsErrored); }
    }

    #endregion Properties

  }

}
