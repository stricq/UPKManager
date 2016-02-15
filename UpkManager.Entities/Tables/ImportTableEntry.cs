using System;
using System.Collections.Generic;


namespace UpkManager.Entities.Tables {

  public class ImportTableEntry : ObjectTableEntry {

    #region Properties

    public NameTableIndex PackageNameIndex { get; set; }

    public NameTableIndex TypeNameIndex { get; set; }
    //
    // OwnerReference in ObjectTableEntry
    //
    // NameTableIndex in ObjectTableEntry
    //
    #endregion Properties

    #region Public Methods

    public int ReadImportTableEntry(byte[] data, int index, List<NameTableEntry> nameTable) {
      string message;

      PackageNameIndex = new NameTableIndex();
      TypeNameIndex    = new NameTableIndex();
      NameIndex        = new NameTableIndex();

      PackageNameIndex.ReadNameTableIndex(data, ref index, nameTable, out message);

      TypeNameIndex.ReadNameTableIndex(data, ref index, nameTable, out message);

      OwnerReference = BitConverter.ToInt32(data, index); index += sizeof(int);

      NameIndex.ReadNameTableIndex(data, ref index, nameTable, out message);

      return index;
    }

    #endregion Public Methods

  }

}
