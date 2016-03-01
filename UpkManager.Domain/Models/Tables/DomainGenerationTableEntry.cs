using UpkManager.Domain.Contracts;


namespace UpkManager.Domain.Models.Tables {

  public class DomainGenerationTableEntry {

    #region Properties

    public int ExportTableCount { get; set; }

    public int NameTableCount { get; set; }

    public int NetObjectCount { get; set; }

    #endregion Properties

    #region Domain Methods

    public void ReadGenerationTableEntry(IByteArrayReader data) {
      ExportTableCount = data.ReadInt32();
      NameTableCount   = data.ReadInt32();
      NetObjectCount   = data.ReadInt32();
    }

    #endregion Domain Methods

  }

}
