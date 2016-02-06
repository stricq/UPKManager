using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;

using STR.MvvmCommon;

using UpkManager.Domain.Models.Compression;
using UpkManager.Domain.Models.Tables;


namespace UpkManager.Domain.Models {

  [Export]
  [PartCreationPolicy(CreationPolicy.Shared)]
  public class DomainHeader : ObservableObject {

    #region Private Fields
    //
    // Entity Fields
    //
    private uint signature;

    private ushort version;
    private ushort licensee;

    private int size;

    private DomainString group;

    private uint flags;

    private int nameTableCount;
    private int nameTableOffset;

    private int exportTableCount;
    private int exportTableOffset;

    private int importTableCount;
    private int importTableOffset;

    private int dependsTableOffset;

    private byte[] guid;

    private ObservableCollection<DomainGenerationTableEntry> generations;

    private uint engineVersion;
    private uint cookerVersion;

    private uint compressionFlags;

    private ObservableCollection<DomainCompressedChunk> compressedChunks;

    private uint unknown1;
    private uint unknown2;

    private ObservableCollection<DomainNameTableEntry> nameTable;

    private ObservableCollection<DomainExportTableEntry> exportTable;
    private ObservableCollection<DomainImportTableEntry> importTable;

    private byte[] dependsTable;
    //
    // Domain Fields
    //
    private string fullFilename;

    #endregion Private Fields

    #region Constructor

    public DomainHeader() {
      guid = new byte[16];

      generations = new ObservableCollection<DomainGenerationTableEntry>();

      compressedChunks = new ObservableCollection<DomainCompressedChunk>();

      nameTable = new ObservableCollection<DomainNameTableEntry>();

      exportTable = new ObservableCollection<DomainExportTableEntry>();
      importTable = new ObservableCollection<DomainImportTableEntry>();
    }

    #endregion Constructor

    #region Properties

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

    public DomainString Group {
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

    public byte[] Guid {
      get { return guid; }
      set { SetField(ref guid, value, () => Guid, () => GuidString); }
    }

    public ObservableCollection<DomainGenerationTableEntry> Generations {
      get { return generations; }
      set { SetField(ref generations, value, () => Generations); }
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

    public ObservableCollection<DomainCompressedChunk> CompressedChunks {
      get { return compressedChunks; }
      set { SetField(ref compressedChunks, value, () => CompressedChunks); }
    }

    public uint Unknown1 {
      get { return unknown1; }
      set { SetField(ref unknown1, value, () => Unknown1); }
    }

    public uint Unknown2 {
      get { return unknown2; }
      set { SetField(ref unknown2, value, () => Unknown2); }
    }

    public ObservableCollection<DomainNameTableEntry> NameTable {
      get { return nameTable; }
      set { SetField(ref nameTable, value, () => NameTable); }
    }

    public ObservableCollection<DomainExportTableEntry> ExportTable {
      get { return exportTable; }
      set { SetField(ref exportTable, value, () => ExportTable); }
    }

    public ObservableCollection<DomainImportTableEntry> ImportTable {
      get { return importTable; }
      set { SetField(ref importTable, value, () => ImportTable); }
    }

    public byte[] DependsTable {
      get { return dependsTable; }
      set { SetField(ref dependsTable, value, () => DependsTable); }
    }

    #endregion Properties

    #region Domain Properties

    public string FullFilename {
      get { return fullFilename; }
      set { SetField(ref fullFilename, value, () => FullFilename, () => Filename); }
    }

    public string Filename => fullFilename.Substring(fullFilename.LastIndexOf(@"\") + 1);

    public string GuidString => new Guid(guid).ToString("B");

    #endregion Domain Properties

  }

}
