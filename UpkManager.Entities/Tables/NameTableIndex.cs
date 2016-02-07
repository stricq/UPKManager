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

    public void ReadNameTableIndex(byte[] data, ref int index, List<NameTableEntry> nameTable) {
      Index   = BitConverter.ToInt32(data, index); index += sizeof(int);
      Numeric = BitConverter.ToInt32(data, index); index += sizeof(int);

      if (Index < 0 || Index > nameTable.Count) Name = "Index Range Error";
      else Name = Numeric > 0 ? $"{nameTable[Index].Name.String}_{Numeric - 1}" : $"{nameTable[Index].Name.String}";
    }

    #endregion Public Methods

  }

}
