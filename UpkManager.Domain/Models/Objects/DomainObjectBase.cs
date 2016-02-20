using STR.MvvmCommon;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Models.Properties;


namespace UpkManager.Domain.Models.Objects {

  public class DomainObjectBase : ObservableObject {

    #region Private Fields

    private bool isExportable;

    private bool isViewable;

    private DomainPropertyHeader propertyHeader;

    private int additionalDataOffset;

    private byte[] additionalData;

    #endregion Private Fields

    #region Properties

    public bool IsExportable {
      get { return isExportable; }
      set { SetField(ref isExportable, value, () => IsExportable); }
    }

    public bool IsViewable {
      get { return isViewable; }
      set { SetField(ref isViewable, value, () => IsViewable); }
    }

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

    #endregion Properties

  }

}
