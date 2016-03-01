using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

using UpkManager.Domain.Contracts;


namespace UpkManager.Domain.Services {

  [Export(typeof(IByteArrayReader))]
  [PartCreationPolicy(CreationPolicy.NonShared)]
  public class ByteArrayReader : IByteArrayReader {

    #region Private Fields

    private byte[] data;

    private int index;

    #endregion Private Fields

    #region IByteArrayReader Implementation

    public void Initialize(byte[] Data, int Index) {
      data = Data;

      if (Index < 0 || Index >= data.Length) throw new ArgumentOutOfRangeException(nameof(Index), "Index value is outside the bounds of the byte array.");

      index = Index;
    }

    public void Seek(int Index) {
      if (Index < 0 || Index >= data.Length) throw new ArgumentOutOfRangeException(nameof(Index), "Index value is outside the bounds of the byte array.");

      index = Index;
    }

    public void Skip(int Count) {
      if (index + Count < 0 || index + Count >= data.Length) throw new ArgumentOutOfRangeException(nameof(Count), "Index + Count is out of the bounds of the byte array.");

      index += Count;
    }

    public IByteArrayReader Splice(int Offset) {
      ByteArrayReader reader = new ByteArrayReader();

      reader.Initialize(data, Offset);

      return reader;
    }

    public async Task<IByteArrayReader> Splice(int Offset, int Length) {
      ByteArrayReader reader = new ByteArrayReader();

      reader.Initialize(await ReadBytes(Offset, Length), 0);

      return reader;
    }

    public async Task DecryptByteArray() {
      if (data.Length < 32) return;

//    const string key = "qiffjdlerdoqymvketdcl0er2subioxq";

      byte[] key = { 0x71, 0x69, 0x66, 0x66, 0x6a, 0x64, 0x6c, 0x65, 0x72, 0x64, 0x6f, 0x71, 0x79, 0x6d, 0x76, 0x6b, 0x65, 0x74, 0x64, 0x63, 0x6c, 0x30, 0x65, 0x72, 0x32, 0x73, 0x75, 0x62, 0x69, 0x6f, 0x78, 0x71 };

      await Task.Run(() => { for(int i = 0; i < data.Length; ++i) data[i] ^= key[i % 32]; });
    }

    public short ReadInt16() {
      short value = BitConverter.ToInt16(data, index); index += sizeof(short);

      return value;
    }

    public ushort ReadUInt16() {
      ushort value = BitConverter.ToUInt16(data, index); index += sizeof(ushort);

      return value;
    }

    public int ReadInt32() {
      int value = BitConverter.ToInt32(data, index); index += sizeof(int);

      return value;
    }

    public uint ReadUInt32() {
      uint value = BitConverter.ToUInt32(data, index); index += sizeof(uint);

      return value;
    }

    public long ReadInt64() {
      long value = BitConverter.ToInt64(data, index); index += sizeof(long);

      return value;
    }

    public ulong ReadUInt64() {
      ulong value = BitConverter.ToUInt64(data, index); index += sizeof(ulong);

      return value;
    }

    public async Task<byte[]> ReadBytes(int length) {
      byte[] value = new byte[length];

      await Task.Run(() => { Array.ConstrainedCopy(data, index, value, 0, length); index += length; });

      return value;
    }

    public async Task<byte[]> ReadBytes(int Offset, int length) {
      byte[] value = new byte[length];

      await Task.Run(() => Array.ConstrainedCopy(data, Offset, value, 0, length));

      return value;
    }

    #endregion IByteArrayReader Implementation

  }

}
