using System;
using System.Diagnostics.CodeAnalysis;

using UpkManager.Entities.Constants;


namespace UpkManager.Entities.PropertyTypes {

  [SuppressMessage("ReSharper", "InconsistentNaming")]
  public class PropertyIntValue : PropertyValueBase {

    #region Private Fields

    protected new int data;

    #endregion Private Fields

    #region Overrides

    public override PropertyType PropertyType => PropertyType.IntProperty;

    public override object Value {
      get { return data; }
      set { data = (int)value; }
    }

    public override void ReadPropertyValue(byte[] Data, ref int Index, UpkHeader header, out string message) {
      message = null;

      data = BitConverter.ToInt32(Data, Index);

      Index += sizeof(uint);
    }

    public override string ToString() {
      return $"{data}";
    }

    #endregion Overrides

  }

}
