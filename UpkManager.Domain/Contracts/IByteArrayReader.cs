

using System.Threading.Tasks;


namespace UpkManager.Domain.Contracts {

  public interface IByteArrayReader {

    /// <summary>
    /// Initializes the ByteArrayReader with an array of bytes and the specified index.
    /// </summary>
    void Initialize(byte[] Data, int Index);

    /// <summary>
    /// Sets the index to the specified value.
    /// </summary>
    void Seek(int Index);

    /// <summary>
    /// Advances the index the specified count of bytes.
    /// </summary>
    void Skip(int Count);

    /// <summary>
    /// Creates a new ByteArrayReader from the current byte array with the index set to the specified offset.
    /// </summary>
    IByteArrayReader Splice(int Offset);

    /// <summary>
    /// Creates a new ByteArrayReader starting at the specified offset with the specified length of data
    /// and the index initialized to 0.
    /// </summary>
    Task<IByteArrayReader> Splice(int Offset, int Length);

    /// <summary>
    /// Decrypts the current byte array from beginning to end.
    /// </summary>
    Task DecryptByteArray();

    short ReadInt16();

    ushort ReadUInt16();

    int ReadInt32();

    uint ReadUInt32();

    long ReadInt64();

    ulong ReadUInt64();

    /// <summary>
    /// Reads the specified number of bytes advancing the index.
    /// </summary>
    Task<byte[]> ReadBytes(int length);

    /// <summary>
    /// Reads the specified number of bytes starting at the specified index.  The current index is not changed.
    /// </summary>
    Task<byte[]> ReadBytes(int Offset, int Length);

  }

}
