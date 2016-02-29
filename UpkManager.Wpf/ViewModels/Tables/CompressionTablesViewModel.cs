using System.Collections.ObjectModel;
using System.ComponentModel.Composition;

using STR.MvvmCommon;

using UpkManager.Wpf.ViewEntities.Tables;


namespace UpkManager.Wpf.ViewModels.Tables {

  [Export]
  [ViewModel("CompressionTablesViewModel")]
  public class CompressionTablesViewModel : ObservableObject {

    #region Private Fields

    private ObservableCollection<CompressionTableEntryViewEntity> compressionTableEntries;

    private ObservableCollection<CompressionBlockViewEntity> compressionBlocks;

    #endregion Private Fields

    #region Properties

    public ObservableCollection<CompressionTableEntryViewEntity> CompressionTableEntries {
      get { return compressionTableEntries; }
      set { SetField(ref compressionTableEntries, value, () => CompressionTableEntries); }
    }

    public ObservableCollection<CompressionBlockViewEntity> CompressionBlocks {
      get { return compressionBlocks; }
      set { SetField(ref compressionBlocks, value, () => CompressionBlocks); }
    }

    #endregion Properties

  }

}
