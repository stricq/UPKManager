using System;

using UpkManager.Entities.Constants;


namespace UpkManager.Entities.ObjectTypes {

  public class ObjectObjectRedirector : ObjectBase {

    #region Properties

    public int ObjectTableReference { get; set; }

    #endregion Properties

    #region Overrides

    public override ObjectType ObjectType => ObjectType.ObjectRedirector;

    public override void ReadUpkObject(byte[] data, ref int index, int endOffset, bool skipProperties, bool skipParse, UpkHeader header, out string message) {
      base.ReadUpkObject(data, ref index, endOffset, skipProperties, skipParse, header, out message);

      if (skipParse) return;

      ObjectTableReference = BitConverter.ToInt32(data, index); index += sizeof(int);
    }

    #endregion Overrides

  }

}
