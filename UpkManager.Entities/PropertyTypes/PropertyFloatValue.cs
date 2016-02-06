using System;
using System.Collections.Generic;

using UpkManager.Entities.Constants;
using UpkManager.Entities.Tables;


namespace UpkManager.Entities.PropertyTypes {

  public class PropertyFloatValue : PropertyValueBase {

    #region Private Fields

    private new float data;

    #endregion Private Fields

    #region Overrides

    public override PropertyType PropertyType => PropertyType.FloatProperty;

    public override object Value {
      get { return data; }
      set { data = (float)value; }
    }

    public override void ReadPropertyValue(byte[] Data, ref int Index, List<NameTableEntry> nameTable) {
      data = BitConverter.ToSingle(Data, Index);

      Index += sizeof(float);
    }

    public override string ToString() {
      return $"{data}";
    }

    #endregion Overrides

  }

}
