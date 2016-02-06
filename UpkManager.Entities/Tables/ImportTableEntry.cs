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

    public void ReadImportTableEntry(byte[] data, ref int index, List<NameTableEntry> nameTable) {
      PackageNameIndex = new NameTableIndex();
      TypeNameIndex    = new NameTableIndex();
      NameIndex        = new NameTableIndex();

      PackageNameIndex.ReadNameTableIndex(data, ref index, nameTable);

      TypeNameIndex.ReadNameTableIndex(data, ref index, nameTable);

      OwnerReference = BitConverter.ToInt32(data, index); index += sizeof(int);

      NameIndex.ReadNameTableIndex(data, ref index, nameTable);
    }

    #endregion Public Methods

  }

}
