using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;

using STR.MvvmCommon;


namespace UpkManager.Wpf.ViewModels {

  [Export]
  [ViewModel("MainMenuViewModel")]
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
  [SuppressMessage("ReSharper", "MemberCanBeInternal")]
  public sealed class MainMenuViewModel : ObservableObject {

    #region Private Fields

    private bool isDdsDefault;
    private bool isDdsFormat1;
    private bool isDdsFormat5;
    private bool isDdsUncompressed;

    private bool isOfflineMode;

    private bool isViewRawData;
    private bool isSkipProperties;
    private bool isSkipParsing;

    private bool isHexViewObject;

    private bool isCompressorRangeFit;
    private bool isCompressorClusterFit;
    private bool isCompressorIterativeFit;

    private bool isErrorMetricUniform;
    private bool isErrorMetricPerceptual;

    private bool isWeightColorByAlpha;

    private bool isWeightingEnabled;

    private RelayCommandAsync reloadFiles;

    private RelayCommandAsync rebuildExported;
    private RelayCommand       deleteExported;

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

    public bool IsDdsDefault {
      get { return isDdsDefault; }
      set { SetField(ref isDdsDefault, value, () => IsDdsDefault); }
    }

    public bool IsDdsFormat1 {
      get { return isDdsFormat1; }
      set { SetField(ref isDdsFormat1, value, () => IsDdsFormat1); }
    }

    public bool IsDdsFormat5 {
      get { return isDdsFormat5; }
      set { SetField(ref isDdsFormat5, value, () => IsDdsFormat5); }
    }

    public bool IsDdsUncompressed {
      get { return isDdsUncompressed; }
      set { SetField(ref isDdsUncompressed, value, () => IsDdsUncompressed); }
    }

    public bool IsOfflineMode {
      get { return isOfflineMode; }
      set { SetField(ref isOfflineMode, value, () => IsOfflineMode); }
    }

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

    public bool IsCompressorRangeFit {
      get { return isCompressorRangeFit; }
      set { SetField(ref isCompressorRangeFit, value, () => IsCompressorRangeFit); }
    }

    public bool IsCompressorClusterFit {
      get { return isCompressorClusterFit; }
      set { SetField(ref isCompressorClusterFit, value, () => IsCompressorClusterFit); }
    }

    public bool IsCompressorIterativeFit {
      get { return isCompressorIterativeFit; }
      set { SetField(ref isCompressorIterativeFit, value, () => IsCompressorIterativeFit); }
    }

    public bool IsErrorMetricUniform {
      get { return isErrorMetricUniform; }
      set { SetField(ref isErrorMetricUniform, value, () => IsErrorMetricUniform); }
    }

    public bool IsErrorMetricPerceptual {
      get { return isErrorMetricPerceptual; }
      set { SetField(ref isErrorMetricPerceptual, value, () => IsErrorMetricPerceptual); }
    }

    public bool IsWeightColorByAlpha {
      get { return isWeightColorByAlpha; }
      set { SetField(ref isWeightColorByAlpha, value, () => IsWeightColorByAlpha); }
    }

    public bool IsWeightingEnabled {
      get { return isWeightingEnabled; }
      set { SetField(ref isWeightingEnabled, value, () => IsWeightingEnabled); }
    }

    public RelayCommandAsync ReloadFiles {
      get { return reloadFiles; }
      set { SetField(ref reloadFiles, value, () => ReloadFiles); }
    }

    public RelayCommandAsync RebuildExported {
      get { return rebuildExported; }
      set { SetField(ref rebuildExported, value, () => RebuildExported); }
    }

    public RelayCommand DeleteExported {
      get { return deleteExported; }
      set { SetField(ref deleteExported, value, () => DeleteExported); }
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
