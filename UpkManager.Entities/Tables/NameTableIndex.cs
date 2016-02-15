using System;
using System.Collections.Generic;


namespace UpkManager.Entities.Tables {

  public class NameTableIndex {

    #region Properties

    public int Index { get; set; }

    public int Numeric { get; set; }

    public string Name { get; set; }

    #endregion Properties

    #region Overrides

    public override string ToString() {
      return Name;
    }

    #endregion Overrides

    #region Public Methods

    public bool ReadNameTableIndex(byte[] data, ref int index, List<NameTableEntry> nameTable, out string message) {
      message = null;

      Index   = BitConverter.ToInt32(data, index); index += sizeof(int);
      Numeric = BitConverter.ToInt32(data, index); index += sizeof(int);

      if (Index < 0 || Index > nameTable.Count) {
        message = $"Index ({Index:X8}) is out of range of the NameTable size.  Current offset is {index:X8}";

        return false;
      }

      Name = Numeric > 0 ? $"{nameTable[Index].Name.String}_{Numeric - 1}" : $"{nameTable[Index].Name.String}";

      return true;
    }

    #endregion Public Methods

  }

}
