using System.ComponentModel.Composition;

using STR.MvvmCommon;

using UpkManager.Domain.Models;


namespace UpkManager.Domain.ViewModels {

  [Export]
  [ViewModel("FileHeaderViewModel")]
  public class FileHeaderViewModel : ObservableObject {

    #region Private Fields

    private DomainHeader header;

    #endregion Private Fields

    #region Properties

    public DomainHeader Header {
      get { return header; }
      set { SetField(ref header, value, () => Header); }
    }

    #endregion Properties

  }

}
