using System.ComponentModel.Composition;

using STR.MvvmCommon;


namespace UpkManager.Domain.ViewModels {

  [Export]
  [ViewModel("MainMenuViewModel")]
  public class MainMenuViewModel : ObservableObject {

    #region Private Fields

    private bool isViewRawData;
    private bool isSkipProperties;
    private bool isSkipParsing;

    private RelayCommandAsync openDirectory;
    private RelayCommandAsync openFile;
    private RelayCommandAsync scanUpkFiles;

    private RelayCommandAsync saveObjectAs;

    private RelayCommand settings;

    private RelayCommand exit;

    #endregion Private Fields

    #region Properties

    public bool IsViewRawData {
      get { return isViewRawData; }
      set { SetField(ref isViewRawData, value, () => IsViewRawData); }
    }

    public bool IsSkipProperties {
      get { return isSkipProperties; }
      set { SetField(ref isSkipProperties, value, () => IsSkipProperties); }
    }

    public bool IsSkipParsing {
      get { return isSkipParsing; }
      set { SetField(ref isSkipParsing, value, () => IsSkipParsing); }
    }

    public RelayCommandAsync OpenDirectory {
      get { return openDirectory; }
      set { SetField(ref openDirectory, value, () => OpenDirectory); }
    }

    public RelayCommandAsync OpenFile {
      get { return openFile; }
      set { SetField(ref openFile, value, () => OpenFile); }
    }

    public RelayCommandAsync ScanUpkFiles {
      get { return scanUpkFiles; }
      set { SetField(ref scanUpkFiles, value, () => ScanUpkFiles); }
    }

    public RelayCommandAsync SaveObjectAs {
      get { return saveObjectAs; }
      set { SetField(ref saveObjectAs, value, () => SaveObjectAs); }
    }

    public RelayCommand Settings {
      get { return settings; }
      set { SetField(ref settings, value, () => Settings); }
    }

    public RelayCommand Exit {
      get { return exit; }
      set { SetField(ref exit, value, () => Exit); }
    }

    #endregion Properties

  }

}
