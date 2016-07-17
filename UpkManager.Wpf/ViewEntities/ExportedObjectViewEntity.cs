using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

using STR.Common.Contracts;

using STR.MvvmCommon;


namespace UpkManager.Wpf.ViewEntities {

  [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
  [SuppressMessage("ReSharper", "MemberCanBeInternal")]
  public class ExportedObjectViewEntity : ObservableObject, ITraversable<ExportedObjectViewEntity> {

    #region Private Fields

    private bool isChecked;
    private bool isSelected;

    private string name;

    private string filename;

    private ExportedObjectViewEntity parent;

    private ObservableCollection<ExportedObjectViewEntity> children;

    #endregion Private Fields

    #region Properties

    public bool IsChecked {
      get { return isChecked; }
      set { SetField(ref isChecked, value, () => IsChecked); }
    }

    public bool IsSelected {
      get { return isSelected; }
      set { SetField(ref isSelected, value, () => IsSelected); }
    }

    public string Name {
      get { return name; }
      set { SetField(ref name, value, () => Name); }
    }

    public string Filename {
      get { return filename; }
      set { SetField(ref filename, value, () => Filename); }
    }

    public ExportedObjectViewEntity Parent {
      get { return parent; }
      set { SetField(ref parent, value, () => Parent); }
    }

    #endregion Properties

    #region ITraversable Implementation

    public ObservableCollection<ExportedObjectViewEntity> Children {
      get { return children; }
      set { SetField(ref children, value, () => Children); }
    }

    #endregion ITraversable Implementation

  }

}
