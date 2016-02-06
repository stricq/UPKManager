using STR.MvvmCommon;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Models.Properties;


namespace UpkManager.Domain.Models.Objects {

  public class DomainObjectBase : ObservableObject {

    #region Private Fields

    private DomainPropertyHeader propertyHeader;

    private int additionalDataOffset;

    private byte[] additionalData;

    private bool canObjectSave;

    #endregion Private Fields

    #region Properties

    public DomainPropertyHeader PropertyHeader {
      get { return propertyHeader; }
      set { SetField(ref propertyHeader, value, () => PropertyHeader); }
    }

    public int AdditionalDataOffset {
      get { return additionalDataOffset; }
      set { SetField(ref additionalDataOffset, value, () => AdditionalDataOffset); }
    }

    public byte[] AdditionalData {
      get { return additionalData; }
      set { SetField(ref additionalData, value, () => AdditionalData); }
    }

    public virtual ObjectType ObjectType => ObjectType.Unknown;

    public bool CanObjectSave {
      get { return canObjectSave; }
      set { SetField(ref canObjectSave, value, () => CanObjectSave); }
    }

    #endregion Properties

  }

}
