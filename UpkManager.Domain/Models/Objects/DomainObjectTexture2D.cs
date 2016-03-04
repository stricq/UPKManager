using System.Collections.ObjectModel;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Models.Objects.Texture2D;


namespace UpkManager.Domain.Models.Objects {

  public class DomainObjectTexture2D : DomainObjectBase {

    #region Properties

    public byte[] Unknown1 { get; set; }

    public int CompressedChunkOffset { get; set; }

    public int CompressedChunkCount { get; set; }

    public ObservableCollection<DomainMipMap> MipMaps { get; set; }

    public byte[] Guid { get; set; }

    #endregion Properties

    #region Overrides

    public override ObjectType ObjectType => ObjectType.Texture2D;

    #endregion Overrides

  }

}
