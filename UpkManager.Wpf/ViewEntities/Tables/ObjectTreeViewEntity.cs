using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

using STR.Common.Contracts;

using STR.MvvmCommon;


namespace UpkManager.Wpf.ViewEntities.Tables {

  [SuppressMessage("ReSharper", "MemberCanBeInternal")]
  public sealed class ObjectTreeViewEntity : ObservableObject, ITraversable<ObjectTreeViewEntity> {

    #region Private Fields

    private bool isExpanded;
    private bool isSelected;

    private bool isImport;
    private bool isExport;

    private int tableIndex;

    private string name;

    private ObjectTreeViewEntity parent;

    private ObservableCollection<ObjectTreeViewEntity> children;

    #endregion Private Fields

    #region Properties

    public ObjectTreeViewEntity Parent {
      get { return parent; }
      set { SetField(ref parent, value, () => Parent); }
    }

    public bool IsExpanded {
      get { return isExpanded; }
      set { SetField(ref isExpanded, value, () => IsExpanded); }
    }

    public bool IsSelected {
      get { return isSelected; }
      set { SetField(ref isSelected, value, () => IsSelected); }
    }

    public bool IsImport {
      get { return isImport; }
      set { SetField(ref isImport, value, () => IsImport); }
    }

    public bool IsExport {
      get { return isExport; }
      set { SetField(ref isExport, value, () => IsExport); }
    }

    public int TableIndex {
      get { return tableIndex; }
      set { SetField(ref tableIndex, value, () => TableIndex); }
    }

    public string Name {
      get { return name; }
      set { SetField(ref name, value, () => Name); }
    }

    #endregion Properties

    #region ITraversable Implementation

    public ObservableCollection<ObjectTreeViewEntity> Children {
      get { return children; }
      set { SetField(ref children, value, () => Children); }
    }

    #endregion ITraversable Implementation

    #region Overrides

    public override string ToString() {
      return Name;
    }

    #endregion Overrides

  }

}
