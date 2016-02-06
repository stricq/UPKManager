using System.ComponentModel.Composition;

using STR.MvvmCommon;


namespace UpkManager.Domain.Models.Compression {

  [Export]
  [PartCreationPolicy(CreationPolicy.Shared)]
  public class DomainCompressedChunkBlock : ObservableObject {

    #region Private Fields
    //
    // Repository Fields
    //
    private int   compressedSize;
    private int uncompressedSize;
    //
    // Domain Fields
    //
    private bool isSelected;

    #endregion Private Fields

    #region Properties

    public int CompressedSize {
      get { return compressedSize; }
      set { SetField(ref compressedSize, value, () => CompressedSize); }
    }

    public int UncompressedSize {
      get { return uncompressedSize; }
      set { SetField(ref uncompressedSize, value, () => UncompressedSize); }
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
