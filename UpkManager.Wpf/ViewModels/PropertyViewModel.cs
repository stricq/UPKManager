using System.ComponentModel.Composition;

using STR.MvvmCommon;

using UpkManager.Domain.Models.Tables;


namespace UpkManager.Wpf.ViewModels {

  [Export]
  [ViewModel("PropertyViewModel")]
  public class PropertyViewModel : ObservableObject {

    #region Private Fields

    private DomainExportTableEntry export;

    #endregion Private Fields

    #region Properties

    public DomainExportTableEntry Export {
      get { return export; }
      set { SetField(ref export, value, () => Export); }
    }

    #endregion Properties

  }

}
