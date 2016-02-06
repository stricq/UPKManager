using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using UpkManager.Entities.Constants;
using UpkManager.Entities.Tables;


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

    public override void ReadPropertyValue(byte[] Data, ref int Index, List<NameTableEntry> nameTable) {
      data = BitConverter.ToInt32(Data, Index);

      Index += sizeof(uint);
    }

    public override string ToString() {
      return $"{data}";
    }

    #endregion Overrides

  }

}
