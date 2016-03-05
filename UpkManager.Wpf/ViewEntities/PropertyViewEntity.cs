using STR.MvvmCommon;


namespace UpkManager.Wpf.ViewEntities {

  public class PropertyViewEntity : ObservableObject {

    #region Private Fields

    private bool isErrored;
    private bool isSelected;

    private int arrayIndex;
    private int size;

    private string name;
    private string typeName;

    private string propertyValue;

    #endregion Private Fields

    #region Properties

    public bool IsErrored {
      get { return isErrored; }
      set { SetField(ref isErrored, value, () => IsErrored); }
    }

    public bool IsSelected {
      get { return isSelected; }
      set { SetField(ref isSelected, value, () => IsSelected); }
    }

    public string Name {
      get { return name; }
      set { SetField(ref name, value, () => Name); }
    }

    public string TypeName {
      get { return typeName; }
      set { SetField(ref typeName, value, () => TypeName); }
    }

    public int Size {
      get { return size; }
      set { SetField(ref size, value, () => Size); }
    }

    public int ArrayIndex {
      get { return arrayIndex; }
      set { SetField(ref arrayIndex, value, () => ArrayIndex); }
    }

    public string PropertyValue {
      get { return propertyValue; }
      set { SetField(ref propertyValue, value, () => PropertyValue); }
    }

    #endregion Properties

  }

}
