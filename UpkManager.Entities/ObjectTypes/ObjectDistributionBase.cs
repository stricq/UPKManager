using System;

using UpkManager.Entities.Constants;


namespace UpkManager.Entities.ObjectTypes {

  public class ObjectDistributionBase : ObjectBase {

    #region Constructor

    public ObjectDistributionBase(ObjectType DistributionType) {
      ObjectType = DistributionType;
    }

    #endregion Constructor

    #region Properties

    public uint Unknown1 { get; set; }

    #endregion Properties

    #region Overrides

    public override ObjectType ObjectType { get; }

    public override void ReadUpkObject(byte[] data, ref int index, int size, bool skipProperties, bool skipParse, UpkHeader header, out string message) {
      //
      // Can't skip this because it is before the properties, not after.
      //
      Unknown1 = BitConverter.ToUInt32(data, index); index += sizeof(uint);

      base.ReadUpkObject(data, ref index, size, skipProperties, skipParse, header, out message);
    }

    #endregion Overrides

  }

}
