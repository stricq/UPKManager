using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Windows;

using STR.MvvmCommon;

using UpkManager.Wpf.ViewEntities;


namespace UpkManager.Wpf.ViewModels {

  [Export]
  [ViewModel("UpkManagerViewModel")]
  [SuppressMessage("ReSharper", "MemberCanBeInternal")]
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  public sealed class UpkManagerViewModel : ObservableObject {

    #region Private Fields

    private RelayCommandAsync<RoutedEventArgs> loaded;

    private RelayCommand<CancelEventArgs> closing;

    private RelayCommand<SizeChangedEventArgs> sizeChanged;

    private SettingsWindowViewEntity settings;

    #endregion Private Fields

    #region Properties

    public RelayCommandAsync<RoutedEventArgs> Loaded {
      get { return loaded; }
      set { SetField(ref loaded, value, () => Loaded); }
    }

    public RelayCommand<CancelEventArgs> Closing {
      get { return closing; }
      set { SetField(ref closing, value, () => Closing); }
    }

    public RelayCommand<SizeChangedEventArgs> SizeChanged {
      get { return sizeChanged; }
      set { SetField(ref sizeChanged, value, () => SizeChanged); }
    }

    public SettingsWindowViewEntity Settings {
      get { return settings; }
      set { SetField(ref settings, value, () => Settings); }
    }

    #endregion Properties

  }

}
