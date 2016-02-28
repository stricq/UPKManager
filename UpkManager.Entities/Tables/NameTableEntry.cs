using System;


namespace UpkManager.Entities.Tables {

  public class NameTableEntry {

    #region Private Fields

    public int TableIndex { get; set; }

    public UpkString Name { get; set; }

    public ulong Flags { get; set; }

    #endregion Private Fields

    #region Overrides

    public override string ToString() {
      return $"{Name.String}";
    }

    #endregion Overrides

    #region Public Methods

    public void ReadNameTableEntry(byte[] data, ref int index) {
      Name = new UpkString();

      Name.ReadUpkStr(data, ref index);

      Flags = BitConverter.ToUInt64(data, index); index += sizeof(ulong);
    }


    #endregion Public Methods

  }

}
