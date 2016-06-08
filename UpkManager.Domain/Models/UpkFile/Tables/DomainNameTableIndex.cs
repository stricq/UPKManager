using System;
using System.Threading.Tasks;

using UpkManager.Domain.Helpers;


namespace UpkManager.Domain.Models.UpkFile.Tables {

  public sealed class DomainNameTableIndex : DomainUpkBuilderBase {

    #region Properties

    public int Index { get; private set; }

    public int Numeric { get; private set; }

    #endregion Properties

    #region Domain Properties

    public string Name { get; set; }

    #endregion Domain Properties

    #region Public Methods

    public void SetNameTableIndex(DomainNameTableEntry nameTableEntry, int numeric = 0) {
      Index = nameTableEntry.TableIndex;

      Numeric = numeric;

      Name = nameTableEntry.Name.String;
    }

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

    public override async Task WriteBuffer(ByteArrayWriter Writer, int CurrentOffset) {
      await Task.Run(() => {
        Writer.WriteInt32(Index);

        Writer.WriteInt32(Numeric);
      });
    }

    #endregion DomainUpkBuilderBase Implementation

  }

}
