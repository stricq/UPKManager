using STR.MvvmCommon;


namespace UpkManager.Domain.Models {

  public class DomainUpkManagerSettings : ObservableObject {

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
