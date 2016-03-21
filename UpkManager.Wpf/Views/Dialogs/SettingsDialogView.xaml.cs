using System.ComponentModel.Composition;
using System.Windows.Controls;

using STR.DialogView.Domain.Contracts;

using STR.MvvmCommon;

using UpkManager.Domain.Constants;

using DialogNames = UpkManager.Wpf.Constants.DialogNames;


namespace UpkManager.Wpf.Views.Dialogs {

  [Export(typeof(IDialogViewLocator))]
  [ViewTag(Name=DialogNames.Settings)]
  public partial class SettingsDialogView : UserControl, IDialogViewLocator {

    public SettingsDialogView() {
      InitializeComponent();
    }

  }

}
