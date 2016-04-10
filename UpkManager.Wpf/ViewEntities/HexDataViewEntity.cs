using System.ComponentModel.Composition;

using STR.MvvmCommon;


namespace UpkManager.Wpf.ViewEntities {

  [Export]
  [PartCreationPolicy(CreationPolicy.NonShared)]
  public class DomainHexData : ObservableObject {

    #region Private Fields

    private bool isErrored;
    private bool isSelected;

    private int fileIndex;

    private int index;

    private string hexValues;

    private string asciiValues;

    #endregion Private Fields

    #region Properties

    public int FileIndex {
      get { return fileIndex; }
      set { SetField(ref fileIndex, value, () => FileIndex); }
    }

    public int Index {
      get { return index; }
      set { SetField(ref index, value, () => Index); }
    }

    public string HexValues {
      get { return hexValues; }
      set { SetField(ref hexValues, value, () => HexValues); }
    }

    public string AsciiValues {
      get { return asciiValues; }
      set { SetField(ref asciiValues, value, () => AsciiValues); }
    }

    #endregion Properties

    #region Domain Properties

    public bool IsErrored {
      get { return isErrored; }
      set { SetField(ref isErrored, value, () => IsErrored); }
    }

    public bool IsSelected {
      get { return isSelected; }
      set { SetField(ref isSelected, value, () => IsSelected); }
    }

    #endregion Domain Properties

  }

}
