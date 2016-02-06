using System;
using System.Collections.Generic;

using UpkManager.Entities.Constants;
using UpkManager.Entities.Tables;


namespace UpkManager.Entities.ObjectTypes {

  public class ObjectDistributionFloatUniform : ObjectBase {

    #region Properties

    public uint Unknown1 { get; set; }

    #endregion Properties

    #region Overrides

    public override ObjectType ObjectType => ObjectType.DistributionFloatUniform;

    public override void ReadUpkObject(byte[] data, ref int index, int size, bool skipProperties, bool skipParse, List<NameTableEntry> nameTable) {
      //
      // Can't skip this because it is before the properties, not after.
      //
      Unknown1 = BitConverter.ToUInt32(data, index); index += sizeof(uint);

      base.ReadUpkObject(data, ref index, size, skipProperties, skipParse, nameTable);
    }

    #endregion Overrides

  }

}
