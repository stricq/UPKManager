using System.Collections.ObjectModel;
using System.ComponentModel.Composition;

using STR.MvvmCommon;


namespace UpkManager.Domain.Models.Compression {

  [Export]
  [PartCreationPolicy(CreationPolicy.Shared)]
  public class DomainCompressedChunkHeader : ObservableObject {

    #region Private Fields
    //
    // Repository Fields
    //
    private uint signature;

    private int blockSize;

    private int   compressedSize;
    private int uncompressedSize;

    private ObservableCollection<DomainCompressedChunkBlock> blocks;
    //
    // Domain Fields
    //
    private bool isSelected;

    #endregion Private Fields

    #region Properties

    public uint Signature {
      get { return signature; }
      set { SetField(ref signature, value, () => Signature); }
    }

    public int BlockSize {
      get { return blockSize; }
      set { SetField(ref blockSize, value, () => BlockSize); }
    }

    public int CompressedSize {
      get { return compressedSize; }
      set { SetField(ref compressedSize, value, () => CompressedSize); }
    }

    public int UncompressedSize {
      get { return uncompressedSize; }
      set { SetField(ref uncompressedSize, value, () => UncompressedSize); }
    }

    public ObservableCollection<DomainCompressedChunkBlock> Blocks {
      get { return blocks; }
      set { SetField(ref blocks, value, () => Blocks); }
    }

    #endregion Properties

    #region Domain Properties

    public bool IsSelected {
      get { return isSelected; }
      set { SetField(ref isSelected, value, () => IsSelected); }
    }

    #endregion Domain Properties

  }

}
