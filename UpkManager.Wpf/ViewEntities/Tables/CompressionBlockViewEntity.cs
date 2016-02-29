using STR.MvvmCommon;


namespace UpkManager.Wpf.ViewEntities.Tables {

  public class CompressionBlockViewEntity : ObservableObject {

    #region Private Fields

    private bool isSelected;

    private int   compressedSize;
    private int uncompressedSize;

    #endregion Private Fields

    #region Properties

    public bool IsSelected {
      get { return isSelected; }
      set { SetField(ref isSelected, value, () => IsSelected); }
    }

    public int CompressedSize {
      get { return compressedSize; }
      set { SetField(ref compressedSize, value, () => CompressedSize); }
    }

    public int UncompressedSize {
      get { return uncompressedSize; }
      set { SetField(ref uncompressedSize, value, () => UncompressedSize); }
    }

    #endregion Properties

  }

}
