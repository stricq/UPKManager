using System.Collections.ObjectModel;
using System.ComponentModel.Composition;

using STR.MvvmCommon;

using UpkManager.Wpf.ViewEntities;


namespace UpkManager.Wpf.ViewModels {

  [Export]
  [ViewModel("PropertyViewModel")]
  public class PropertyViewModel : ObservableObject {

    #region Private Fields

    private ObservableCollection<PropertyViewEntity> properties;

    #endregion Private Fields

    #region Properties

    public ObservableCollection<PropertyViewEntity> Properties {
      get { return properties; }
      set { SetField(ref properties, value, () => Properties); }
    }

    #endregion Properties

  }

}
