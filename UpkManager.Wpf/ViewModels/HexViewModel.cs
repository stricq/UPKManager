using System.Collections.ObjectModel;
using System.ComponentModel.Composition;

using STR.MvvmCommon;

using UpkManager.Wpf.ViewEntities;


namespace UpkManager.Wpf.ViewModels {

  [Export]
  [ViewModel("HexViewModel")]
  public class HexViewModel : ObservableObject {

    #region Private Fields

    private RelayCommand searchBackward;
    private RelayCommand searchForward;

    private string searchText;

    private string title;

    private ObservableCollection<DomainHexData> hexData;

    #endregion Private Fields

    #region Properties

    public RelayCommand SearchBackward {
      get { return searchBackward; }
      set { SetField(ref searchBackward, value, () => SearchBackward); }
    }

    public RelayCommand SearchForward {
      get { return searchForward; }
      set { SetField(ref searchForward, value, () => SearchForward); }
    }

    public string SearchText {
      get { return searchText; }
      set { SetField(ref searchText, value, () => SearchText); }
    }

    public string Title {
      get { return title; }
      set { SetField(ref title, value, () => Title); }
    }

    public ObservableCollection<DomainHexData> HexData {
      get { return hexData; }
      set { SetField(ref hexData, value, () => HexData); }
    }

    #endregion Properties

  }

}
