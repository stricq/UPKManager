using System.Collections.Generic;

using UpkManager.Entities.Constants;
using UpkManager.Entities.Tables;


namespace UpkManager.Entities.PropertyTypes {

  public class PropertyStructValue : PropertyValueBase {

    #region Properties

    public NameTableIndex StructNameIndex;

    #endregion Properties

    #region Overrides

    public override PropertyType PropertyType => PropertyType.StructProperty;

    public override void ReadPropertyValue(byte[] Data, ref int Index, List<NameTableEntry> nameTable) {
      StructNameIndex = new NameTableIndex();

      StructNameIndex.ReadNameTableIndex(Data, ref Index, nameTable);

      base.ReadPropertyValue(Data, ref Index, nameTable);
    }

    public override string ToString() {
      return $"{StructNameIndex.Name}";
    }

    #endregion Overrides

  }

}
