using System;

using STR.MvvmCommon;


namespace UpkManager.Wpf.ViewEntities {

  public class HeaderViewEntity : ObservableObject {

    #region Private Fields

    private string fullFilename;

    private uint signature;

    private ushort version;
    private ushort licensee;

    private int size;

    private string group;

    private uint flags;

    private int nameTableCount;
    private int nameTableOffset;

    private int exportTableCount;
    private int exportTableOffset;

    private int importTableCount;
    private int importTableOffset;

    private int dependsTableOffset;

    private Guid guid;

    private int generationsCount;

    private uint engineVersion;
    private uint cookerVersion;

    private uint compressionFlags;

    private int compressedChunksCount;

    private uint unknown1;
    private uint unknown2;

    #endregion Private Fields

    #region Properties

    public string FullFilename {
      get { return fullFilename; }
      set { SetField(ref fullFilename, value, () => FullFilename); }
    }

    public uint Signature {
      get { return signature; }
      set { SetField(ref signature, value, () => Signature); }
    }

    public ushort Version {
      get { return version; }
      set { SetField(ref version, value, () => Version); }
    }

    public ushort Licensee {
      get { return licensee; }
      set { SetField(ref licensee, value, () => Licensee); }
    }

    public int Size {
      get { return size; }
      set { SetField(ref size, value, () => Size); }
    }

    public string Group {
      get { return group; }
      set { SetField(ref group, value, () => Group); }
    }

    public uint Flags {
      get { return flags; }
      set { SetField(ref flags, value, () => Flags); }
    }

    public int NameTableCount {
      get { return nameTableCount; }
      set { SetField(ref nameTableCount, value, () => NameTableCount); }
    }

    public int NameTableOffset {
      get { return nameTableOffset; }
      set { SetField(ref nameTableOffset, value, () => NameTableOffset); }
    }

    public int ExportTableCount {
      get { return exportTableCount; }
      set { SetField(ref exportTableCount, value, () => ExportTableCount); }
    }

    public int ExportTableOffset {
      get { return exportTableOffset; }
      set { SetField(ref exportTableOffset, value, () => ExportTableOffset); }
    }

    public int ImportTableCount {
      get { return importTableCount; }
      set { SetField(ref importTableCount, value, () => ImportTableCount); }
    }

    public int ImportTableOffset {
      get { return importTableOffset; }
      set { SetField(ref importTableOffset, value, () => ImportTableOffset); }
    }

    public int DependsTableOffset {
      get { return dependsTableOffset; }
      set { SetField(ref dependsTableOffset, value, () => DependsTableOffset); }
    }

    public Guid Guid {
      get { return guid; }
      set { SetField(ref guid, value, () => Guid); }
    }

    public int GenerationTableCount {
      get { return generationsCount; }
      set { SetField(ref generationsCount, value, () => GenerationTableCount); }
    }

    public uint EngineVersion {
      get { return engineVersion; }
      set { SetField(ref engineVersion, value, () => EngineVersion); }
    }

    public uint CookerVersion {
      get { return cookerVersion; }
      set { SetField(ref cookerVersion, value, () => CookerVersion); }
    }

    public uint CompressionFlags {
      get { return compressionFlags; }
      set { SetField(ref compressionFlags, value, () => CompressionFlags); }
    }

    public int CompressionTableCount {
      get { return compressedChunksCount; }
      set { SetField(ref compressedChunksCount, value, () => CompressionTableCount); }
    }

    public uint Unknown1 {
      get { return unknown1; }
      set { SetField(ref unknown1, value, () => Unknown1); }
    }

    public uint Unknown2 {
      get { return unknown2; }
      set { SetField(ref unknown2, value, () => Unknown2); }
    }

    #endregion Properties

  }

}
