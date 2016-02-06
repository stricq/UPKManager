using System;
using System.Collections.Generic;

using UpkManager.Entities.Constants;
using UpkManager.Entities.Tables;


namespace UpkManager.Entities.PropertyTypes {

  public class PropertyGuidValue : PropertyValueBase {

    #region Overrides

    public override PropertyType PropertyType => PropertyType.GuidProperty;

    public override void ReadPropertyValue(byte[] Data, ref int Index, List<NameTableEntry> nameTable) {
      data = new byte[16];

      Array.ConstrainedCopy(Data, Index, data, 0, 16);

      Index += 16;
    }

    public override string ToString() {
      Guid guid = new Guid(data);

      return $"{guid.ToString("B")}";
    }

    #endregion Overrides

  }

}
