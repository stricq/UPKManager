using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using UpkManager.Entities.Constants;
using UpkManager.Entities.Tables;


namespace UpkManager.Entities.PropertyTypes {

  [SuppressMessage("ReSharper", "InconsistentNaming")]
  public class PropertyNameValue : PropertyValueBase {

    #region Protected Fields

    protected new NameTableIndex data;

    #endregion Protected Fields

    #region Overrides

    public override PropertyType PropertyType => PropertyType.NameProperty;

    public override object Value {
      get { return data; }
      set { data = (NameTableIndex)value; }
    }

    public override void ReadPropertyValue(byte[] Data, ref int Index, UpkHeader header, out string message) {
      data = new NameTableIndex();

      data.ReadNameTableIndex(Data, ref Index, header.NameTable, out message);
    }

    public override string ToString() {
      return $"{data.Name}";
    }

    #endregion Overrides

  }

}
