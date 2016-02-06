using System.Collections.ObjectModel;
using System.ComponentModel.Composition;

using STR.MvvmCommon;

using UpkManager.Domain.Models.Compression;
using UpkManager.Domain.Models.Tables;


namespace UpkManager.Domain.ViewModels {

  [Export]
  [ViewModel("HeaderTablesViewModel")]
  public class HeaderTablesViewModel : ObservableObject {

    #region Private Fields

    private bool isViewCleanData = true;

    private bool isViewRawData;

    private ObservableCollection<DomainGenerationTableEntry> generations;

    private ObservableCollection<DomainCompressedChunk> chunks;

    private ObservableCollection<DomainCompressedChunkBlock> blocks;

    #endregion Private Fields

    #region Properties

    public bool IsViewCleanData {
      get { return isViewCleanData; }
      set { SetField(ref isViewCleanData, value, () => IsViewCleanData); }
    }

    public bool IsViewRawData {
      get { return isViewRawData; }
      set { SetField(ref isViewRawData, value, () => IsViewRawData); }
    }

    public ObservableCollection<DomainGenerationTableEntry> Generations {
      get { return generations; }
      set { SetField(ref generations, value, () => Generations); }
    }

    public ObservableCollection<DomainCompressedChunk> Chunks {
      get { return chunks; }
      set { SetField(ref chunks, value, () => Chunks); }
    }

    public ObservableCollection<DomainCompressedChunkBlock> Blocks {
      get { return blocks; }
      set { SetField(ref blocks, value, () => Blocks); }
    }

    #endregion Properties

  }

}
