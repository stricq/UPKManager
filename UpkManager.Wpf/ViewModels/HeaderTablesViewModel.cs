using System.ComponentModel.Composition;

using STR.MvvmCommon;


namespace UpkManager.Wpf.ViewModels {

  [Export]
  [ViewModel("HeaderTablesViewModel")]
  public class HeaderTablesViewModel : ObservableObject {

    #region Private Fields

    private bool isViewCleanData = true;

    private bool isViewRawData;

    #endregion Private Fields

    #region Properties

    public bool IsViewCleanData {
      get { return isViewCleanData; }
      set { SetField(ref isViewCleanData, value, () => IsViewCleanData); }
    }

    public bool IsViewRawData {
      get { return isViewRawData; }
      set { SetField(ref isViewRawData, value, () => IsViewRawData); }
    }

    #endregion Properties

  }

}
