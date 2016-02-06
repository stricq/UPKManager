using System.ComponentModel.Composition;

using STR.MvvmCommon;


namespace UpkManager.Domain.Models.Tables {

  [Export]
  [PartCreationPolicy(CreationPolicy.Shared)]
  public class DomainNameTableEntry : ObservableObject {

    #region Private Fields
    //
    // Repository Fields
    //
    private int index;

    private DomainString name;

    private ulong flags;
    //
    // Domain Fields
    //
    private bool isSelected;

    #endregion Private Fields

    #region Properties

    public int Index {
      get { return index; }
      set { SetField(ref index, value, () => Index); }
    }

    public DomainString Name {
      get { return name; }
      set { SetField(ref name, value, () => Name); }
    }

    public ulong Flags {
      get { return flags; }
      set { SetField(ref flags, value, () => Flags); }
    }

    #endregion Properties

    #region Domain Properties

    public bool IsSelected {
      get { return isSelected; }
      set { SetField(ref isSelected, value, () => IsSelected); }
    }

    public string NameString => name?.String;

    #endregion Domain Properties

  }

}
