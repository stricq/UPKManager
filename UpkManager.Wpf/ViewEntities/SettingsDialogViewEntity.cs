using STR.MvvmCommon;


namespace UpkManager.Wpf.ViewEntities {

  public class SettingsDialogViewEntity : ObservableObject {

    #region Private Fields

    private string pathToGame;

    private string exportPath;

    #endregion Private Fields

    #region Properties

    public string PathToGame {
      get { return pathToGame; }
      set { SetField(ref pathToGame, value, () => PathToGame); }
    }

    public string ExportPath {
      get { return exportPath; }
      set { SetField(ref exportPath, value, () => ExportPath); }
    }

    #endregion Properties

  }

}
