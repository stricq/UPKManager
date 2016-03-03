using System;

using UpkManager.Domain.Contracts;


namespace UpkManager.Domain.Models.Tables {

  public class DomainNameTableIndex {

    #region Properties

    public int Index { get; set; }

    public int Numeric { get; set; }

    public string Name { get; set; }

    #endregion Properties

    #region Public Methods

    public void ReadNameTableIndex(IByteArrayReader reader, DomainHeader header) {
      Index   = reader.ReadInt32();
      Numeric = reader.ReadInt32();

      if (Index < 0 || Index > header.NameTable.Count) throw new ArgumentOutOfRangeException(nameof(Index), $"Index ({Index:X8}) is out of range of the NameTable size.");

      Name = Numeric > 0 ? $"{header.NameTable[Index].Name.String}_{Numeric - 1}" : $"{header.NameTable[Index].Name.String}";
    }

    #endregion Public Methods

  }

}
