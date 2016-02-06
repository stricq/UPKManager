using STR.MvvmCommon;

using UpkManager.Domain.Models.Tables;


namespace UpkManager.Domain.Models.Properties {

  public class DomainProperty : ObservableObject {

    #region Private Fields

    private DomainNameTableIndex nameIndex;

    private DomainNameTableIndex typeNameIndex;

    private int size;

    private int arrayIndex;

    private DomainPropertyValueBase value;

    #endregion Private Fields

    #region Properties

    public DomainNameTableIndex NameIndex {
      get { return nameIndex; }
      set { SetField(ref nameIndex, value, () => NameIndex); }
    }

    public DomainNameTableIndex TypeNameIndex {
      get { return typeNameIndex; }
      set { SetField(ref typeNameIndex, value, () => TypeNameIndex); }
    }

    public int Size {
      get { return size; }
      set { SetField(ref size, value, () => Size); }
    }

    public int ArrayIndex {
      get { return arrayIndex; }
      set { SetField(ref arrayIndex, value, () => ArrayIndex); }
    }

    public DomainPropertyValueBase Value {
      get { return value; }
      set { SetField(ref this.value, value, () => Value); }
    }

    #endregion Properties

    #region Domain Properties

    public string Name => nameIndex.Name;

    public string TypeName => typeNameIndex?.Name;

    #endregion Domain Properties

  }

}
