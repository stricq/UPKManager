using System;
using System.Collections.Generic;

using UpkManager.Entities.Constants;
using UpkManager.Entities.Tables;


namespace UpkManager.Entities.PropertyTypes {

  public class PropertyBoolValue : PropertyValueBase {

    #region Private Fields

    private new uint data;

    #endregion Private Fields

    #region Overrides

    public override PropertyType PropertyType => PropertyType.BoolProperty;

    public override object Value {
      get { return data; }
      set { data = (uint)value; }
    }

    public override void ReadPropertyValue(byte[] Data, ref int Index, List<NameTableEntry> nameTable) {
      data = BitConverter.ToUInt32(Data, Index);

      Index += sizeof(uint);
    }

    public override string ToString() {
      return $"{data != 0}";
    }

    #endregion Overrides

  }

}
