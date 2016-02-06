using System.ComponentModel.Composition;

using STR.MvvmCommon;


namespace UpkManager.Domain.Models.Tables {

  [Export]
  [PartCreationPolicy(CreationPolicy.NonShared)]
  public class DomainNameTableIndex : ObservableObject {

    #region Private Fields

    private int index;

    private int numeric;

    private string name;

    #endregion Private Fields

    #region Properties

    public int Index {
      get { return index; }
      set { SetField(ref index, value, () => Index); }
    }

    public int Numeric {
      get { return numeric; }
      set { SetField(ref numeric, value, () => Numeric); }
    }

    public string Name {
      get { return name; }
      set { SetField(ref name, value, () => Name); }
    }

    #endregion Properties

  }

}
