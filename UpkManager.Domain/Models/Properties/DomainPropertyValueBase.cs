using System.ComponentModel.Composition;

using UpkManager.Domain.Constants;


namespace UpkManager.Domain.Models.Properties {

  [Export]
  [PartCreationPolicy(CreationPolicy.NonShared)]
  public class DomainPropertyValueBase {

    #region Private Fields

    protected byte[] data; // propertySize bytes of data

    #endregion Private Fields

    #region Properties

    public virtual PropertyType PropertyType => PropertyType.UnknownProperty;

    public virtual object Value {
      get { return data; }
      set { data = (byte[])value; }
    }

    #endregion Properties

    #region Methods

    public override string ToString() {
      return $"{data.Length:N0} Bytes of Data";
    }

    #endregion Methods

  }

}
