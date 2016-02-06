using System.Collections.ObjectModel;
using System.ComponentModel.Composition;

using STR.Common.Contracts;
using STR.MvvmCommon;


namespace UpkManager.Domain.ViewModels {

  [Export]
  [PartCreationPolicy(CreationPolicy.NonShared)]
  public class UpkFileInfo : ObservableObject, ITraversable<UpkFileInfo> {

    #region Private Fields

    private bool isChecked;

    private int fileSize;

    private string filename;
    private string hasTextures;

    private ObservableCollection<UpkFileInfo> children;

    #endregion Private Fields

    #region Properties

    public bool IsChecked {
      get { return isChecked; }
      set { SetField(ref isChecked, value, () => IsChecked); }
    }

    public int FileSize {
      get { return fileSize; }
      set { SetField(ref fileSize, value, () => FileSize); }
    }

    public string Filename {
      get { return filename; }
      set { SetField(ref filename, value, () => Filename); }
    }

    public string HasTextures {
      get { return hasTextures; }
      set { SetField(ref hasTextures, value, () => HasTextures); }
    }

    public ObservableCollection<UpkFileInfo> Children {
      get { return children; }
      set { SetField(ref children, value, () => Children); }
    }

    #endregion Properties

  }

}
