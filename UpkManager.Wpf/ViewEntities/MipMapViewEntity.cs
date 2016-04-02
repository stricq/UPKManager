using STR.MvvmCommon;


namespace UpkManager.Wpf.ViewEntities {

  public class MipMapViewEntity : ObservableObject {

    #region Private Fields

    private bool isChecked;
    private bool isEnabled;

    private int level;
    private int width;
    private int height;

    #endregion Private Fields

    #region Properties

    public bool IsChecked {
      get { return isChecked; }
      set { SetField(ref isChecked, value, () => IsChecked); }
    }

    public bool IsEnabled {
      get { return isEnabled; }
      set { SetField(ref isEnabled, value, () => IsEnabled); }
    }

    public int Level {
      get { return level; }
      set { SetField(ref level, value, () => Level, () => LevelSize); }
    }

    public int Width {
      get { return width; }
      set { SetField(ref width, value, () => Width); }
    }

    public int Height {
      get { return height; }
      set { SetField(ref height, value, () => Height); }
    }

    public string LevelSize => $"{Level} - {Width}x{Height}";

    #endregion Properties

  }

}
