using System.ComponentModel.Composition;

using STR.MvvmCommon;


namespace UpkManager.Domain.Models {

  [Export]
  [PartCreationPolicy(CreationPolicy.NonShared)]
  public class DomainString : ObservableObject {

    #region Private Fields

    private int size;

    private string @string;

    #endregion Private Fields

    #region Properties

    public int Size {
      get { return size; }
      set { SetField(ref size, value, () => Size); }
    }

    public string String {
      get { return @string; }
      set { SetField(ref @string, value, () => String); }
    }

    #endregion Properties

  }

}
