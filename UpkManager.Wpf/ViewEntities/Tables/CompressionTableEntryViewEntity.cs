using System.Collections.ObjectModel;

using STR.MvvmCommon;


namespace UpkManager.Wpf.ViewEntities.Tables {

  public class CompressionTableEntryViewEntity : ObservableObject {

    #region Private Fields

    private bool isErrored;
    private bool isSelected;

    private int uncompressedOffset;
    private int uncompressedSize;

    private int compressedOffset;
    private int compressedSize;

    private int blockSize;

    private ObservableCollection<CompressionBlockViewEntity> compressionBlocks;

    #endregion Private Fields

    #region Properties

    public bool IsErrored {
      get { return isErrored; }
      set { SetField(ref isErrored, value, () => IsErrored); }
    }

    public bool IsSelected {
      get { return isSelected; }
      set { SetField(ref isSelected, value, () => IsSelected); }
    }

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

    public int BlockSize {
      get { return blockSize; }
      set { SetField(ref blockSize, value, () => BlockSize); }
    }

    public ObservableCollection<CompressionBlockViewEntity> CompressionBlocks {
      get { return compressionBlocks; }
      set { SetField(ref compressionBlocks, value, () => CompressionBlocks); }
    }

    #endregion Properties

  }

}
