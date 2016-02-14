using STR.MvvmCommon;


namespace UpkManager.Domain.Models {

  public class DomainUpkManagerSettings : ObservableObject {

    #region Private Fields

    private string pathToGame;

    #endregion Private Fields

    #region Properties

    public string PathToGame {
      get { return pathToGame; }
      set { SetField(ref pathToGame, value, () => PathToGame); }
    }

    #endregion Properties

  }

}
