using System;
using System.Threading.Tasks;


namespace UpkManager.Domain.Helpers {

  public class ByteArrayWriter {

    #region Private Fields

    private byte[] data;

    private int index;

    #endregion Private Fields

    #region Public Methods

    public byte[] GetBytes() {
      return data;
    }

    public int Index => index;

    public static ByteArrayWriter CreateNew(int Length) {
      ByteArrayWriter writer = new ByteArrayWriter();

      byte[] data = new byte[Length];

      writer.Initialize(data, 0);

      return writer;
    }

    public void Initialize(byte[] Data, int Index) {
      data = Data;

      if (Index < 0 || Index > data.Length) throw new ArgumentOutOfRangeException(nameof(Index), "Index value is outside the bounds of the byte array.");

      index = Index;
    }

    public void Seek(int Offset) {
      if (Offset < 0 || Offset > data.Length) throw new ArgumentOutOfRangeException(nameof(Offset), "Index value is outside the bounds of the byte array.");

      index = Offset;
    }

    public void WriteByte(byte Value) {
      data[index] = Value; ++index;
    }

    public void WriteInt16(short Value) {
      byte[] bytes = BitConverter.GetBytes(Value);

      Array.ConstrainedCopy(bytes, 0, data, index, sizeof(short)); index += sizeof(short);
    }

    public void WriteUInt16(ushort Value) {
      byte[] bytes = BitConverter.GetBytes(Value);

      Array.ConstrainedCopy(bytes, 0, data, index, sizeof(ushort)); index += sizeof(ushort);
    }

    public void WriteInt32(int Value) {
      byte[] bytes = BitConverter.GetBytes(Value);

      Array.ConstrainedCopy(bytes, 0, data, index, sizeof(int)); index += sizeof(int);
    }

    public void WriteUInt32(uint Value) {
      byte[] bytes = BitConverter.GetBytes(Value);

      Array.ConstrainedCopy(bytes, 0, data, index, sizeof(uint)); index += sizeof(uint);
    }

    public void WriteInt64(long Value) {
      byte[] bytes = BitConverter.GetBytes(Value);

      Array.ConstrainedCopy(bytes, 0, data, index, sizeof(long)); index += sizeof(long);
    }

    public void WriteUInt64(ulong Value) {
      byte[] bytes = BitConverter.GetBytes(Value);

      Array.ConstrainedCopy(bytes, 0, data, index, sizeof(ulong)); index += sizeof(ulong);
    }

    public void WriteSingle(float Value) {
      byte[] bytes = BitConverter.GetBytes(Value);

      Array.ConstrainedCopy(bytes, 0, data, index, sizeof(float)); index += sizeof(float);
    }

    public async Task WriteBytes(byte[] Bytes) {
      if (Bytes == null || Bytes.Length == 0) return;

      if (Bytes.Length + index > data.Length) throw new ArgumentOutOfRangeException(nameof(Bytes), "Current Index + Bytes.Length is out of bounds of the byte array.");

      await Task.Run(() => Array.ConstrainedCopy(Bytes, 0, data, index, Bytes.Length));

      index += Bytes.Length;
    }

    #endregion Public Methods

  }

}
