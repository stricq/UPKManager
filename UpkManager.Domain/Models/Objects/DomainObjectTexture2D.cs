

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Models.Objects.Texture2D;


namespace UpkManager.Domain.Models.Objects {

  public class DomainObjectTexture2D : DomainObjectBase {

    #region Private Fields

    private byte[] unknown1;

    private int compressedChunkOffset;
    private int compressedChunkCount;


    private ObservableCollection<DomainMipMap> mipMaps;

    private byte[] guid;

    #endregion Private Fields

    #region Properties

    public byte[] Unknown1 {
      get { return unknown1; }
      set { SetField(ref unknown1, value, () => Unknown1); }
    }

    public int CompressedChunkOffset {
      get { return compressedChunkOffset; }
      set { SetField(ref compressedChunkOffset, value, () => CompressedChunkOffset); }
    }

    public int CompressedChunkCount {
      get { return compressedChunkCount; }
      set { SetField(ref compressedChunkCount, value, () => CompressedChunkCount); }
    }

    public ObservableCollection<DomainMipMap> MipMaps {
      get { return mipMaps; }
      set { SetField(ref mipMaps, value, () => MipMaps); }
    }

    public byte[] Guid {
      get { return guid; }
      set { SetField(ref guid, value, () => Guid); }
    }

    #endregion Properties

    #region Domain Properties

    public string GuidString => new Guid(guid).ToString("B");

    #endregion Domain Properties

    #region Overrides

    public override ObjectType ObjectType => ObjectType.Texture2D;

    #endregion Overrides

  }

}
