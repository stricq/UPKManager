using System.ComponentModel.Composition;

using STR.MvvmCommon;


namespace UpkManager.Domain.Models.Compression {

  [Export]
  [PartCreationPolicy(CreationPolicy.Shared)]
  public class DomainCompressedChunk : ObservableObject {

    #region Private Fields
    //
    // Repository Fields
    //
    private int uncompressedOffset;
    private int uncompressedSize;

    private int compressedOffset;
    private int compressedSize;

    private DomainCompressedChunkHeader header;
    //
    // Domain Fields
    //
    private bool isSelected;

    #endregion Private Fields

    #region Properties

    public int UncompressedOffset {
      get { return uncompressedOffset; }
      set { SetField(ref uncompressedOffset, value, () => UncompressedOffset); }
    }

    public int UncompressedSize {
      get { return uncompressedSize; }
      set { SetField(ref uncompressedSize, value, () => UncompressedSize); }
    }

    public int CompressedOffset {
      get { return compressedOffset; }
      set { SetField(ref compressedOffset, value, () => CompressedOffset); }
    }

    public int CompressedSize {
      get { return compressedSize; }
      set { SetField(ref compressedSize, value, () => CompressedSize); }
    }

    public DomainCompressedChunkHeader Header {
      get { return header; }
      set { SetField(ref header, value, () => Header); }
    }

    #endregion Properties

    #region Domain Properties

    public bool IsSelected {
      get { return isSelected; }
      set { SetField(ref isSelected, value, () => IsSelected); }
    }

    #endregion Domain Properties

  }

}
