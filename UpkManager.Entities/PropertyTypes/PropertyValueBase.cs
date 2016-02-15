using System;
using System.Diagnostics.CodeAnalysis;

using UpkManager.Entities.Constants;


namespace UpkManager.Entities.PropertyTypes {

  [SuppressMessage("ReSharper", "InconsistentNaming")]
  public class PropertyValueBase {

    #region Private Fields

    protected byte[] data;

    #endregion Private Fields

    #region Properties

    public int Size { get; set; }

    #endregion Properties

    #region Virtual Properties

    public virtual PropertyType PropertyType => PropertyType.UnknownProperty;

    public virtual object Value {
      get { return data; }
      set { data = (byte[])value; }
    }

    #endregion Virtual Properties

    #region Virtual Methods

    public virtual void ReadPropertyValue(byte[] Data, ref int Index, UpkHeader header, out string message) {
      message = null;

      data = new byte[Size];

      Array.ConstrainedCopy(Data, Index, data, 0, Size);

      Index += Size;
    }

    #endregion Virtual Methods

    #region Overrides

    public override string ToString() {
      return $"{data.Length:N0} Bytes of Data";
    }

    #endregion Overrides

  }

}
