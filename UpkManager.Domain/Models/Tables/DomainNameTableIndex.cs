using System;

using UpkManager.Domain.Helpers;


namespace UpkManager.Domain.Models.Tables {

  public class DomainNameTableIndex : DomainUpkBuilderBase {

    #region Properties

    public int Index { get; set; }

    public int Numeric { get; set; }

    #endregion Properties

    #region Domain Properties

    public string Name { get; set; }

    #endregion Domain Properties

    #region Public Methods

    public void ReadNameTableIndex(ByteArrayReader reader, DomainHeader header) {
      Index   = reader.ReadInt32();
      Numeric = reader.ReadInt32();

      if (Index < 0 || Index > header.NameTable.Count) throw new ArgumentOutOfRangeException(nameof(Index), $"Index ({Index:X8}) is out of range of the NameTable size.");

      Name = Numeric > 0 ? $"{header.NameTable[Index].Name.String}_{Numeric - 1}" : $"{header.NameTable[Index].Name.String}";
    }

    #endregion Public Methods

    #region DomainUpkBuilderBase Implementation

    public override int GetBuilderSize() {
      BuilderSize = sizeof(int) * 2;

      return BuilderSize;
    }

    #endregion DomainUpkBuilderBase Implementation

  }

}
