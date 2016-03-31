using System.ComponentModel.Composition;

using STR.MvvmCommon;


namespace UpkManager.Wpf.ViewModels {

  [Export]
  [ViewModel("MainMenuViewModel")]
  public class MainMenuViewModel : ObservableObject {

    #region Private Fields

    private bool isViewRawData;
    private bool isSkipProperties;
    private bool isSkipParsing;

    private bool isHexViewObject;

    private RelayCommand scanUpkFiles;

    private RelayCommandAsync exportHexView;

    private RelayCommandAsync exportFiles;
    private RelayCommandAsync saveObjectAs;

    private RelayCommand selectAllFiles;
    private RelayCommand deselectAllFiles;

    private RelayCommand settings;
    private RelayCommand about;

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

    public bool IsHexViewObject {
      get { return isHexViewObject; }
      set { SetField(ref isHexViewObject, value, () => IsHexViewObject); }
    }

    public RelayCommand ScanUpkFiles {
      get { return scanUpkFiles; }
      set { SetField(ref scanUpkFiles, value, () => ScanUpkFiles); }
    }

    public RelayCommandAsync ExportHexView {
      get { return exportHexView; }
      set { SetField(ref exportHexView, value, () => ExportHexView); }
    }

    public RelayCommandAsync ExportFiles {
      get { return exportFiles; }
      set { SetField(ref exportFiles, value, () => ExportFiles); }
    }

    public RelayCommandAsync SaveObjectAs {
      get { return saveObjectAs; }
      set { SetField(ref saveObjectAs, value, () => SaveObjectAs); }
    }

    public RelayCommand SelectAllFiles {
      get { return selectAllFiles; }
      set { SetField(ref selectAllFiles, value, () => SelectAllFiles); }
    }

    public RelayCommand DeselectAllFiles {
      get { return deselectAllFiles; }
      set { SetField(ref deselectAllFiles, value, () => DeselectAllFiles); }
    }

    public RelayCommand Settings {
      get { return settings; }
      set { SetField(ref settings, value, () => Settings); }
    }

    public RelayCommand About {
      get { return about; }
      set { SetField(ref about, value, () => About); }
    }

    public RelayCommand Exit {
      get { return exit; }
      set { SetField(ref exit, value, () => Exit); }
    }

    #endregion Properties

  }

}
