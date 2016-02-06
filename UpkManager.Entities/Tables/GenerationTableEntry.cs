using System;


namespace UpkManager.Entities.Tables {

  public class GenerationTableEntry {

    #region Properties

    public int ExportTableCount { get; set; }

    public int NameTableCount { get; set; }

    public int NetObjectCount { get; set; }

    #endregion Properties

    #region Public Methods

    public void ReadGenerationTableEntry(byte[] data, ref int index) {
      ExportTableCount = BitConverter.ToInt32(data, index); index += sizeof(int);
      NameTableCount   = BitConverter.ToInt32(data, index); index += sizeof(int);
      NetObjectCount   = BitConverter.ToInt32(data, index); index += sizeof(int);
    }

    #endregion Public Methods

  }

}
