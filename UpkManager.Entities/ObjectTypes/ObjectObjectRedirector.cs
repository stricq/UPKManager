using System;
using System.Collections.Generic;

using UpkManager.Entities.Constants;
using UpkManager.Entities.Tables;


namespace UpkManager.Entities.ObjectTypes {

  public class ObjectObjectRedirector : ObjectBase {

    #region Properties

    public int ObjectTableReference { get; set; }

    #endregion Properties

    #region Overrides

    public override ObjectType ObjectType => ObjectType.ObjectRedirector;

    public override void ReadUpkObject(byte[] data, ref int index, int endOffset, bool skipProperties, bool skipParse, List<NameTableEntry> nameTable) {
      base.ReadUpkObject(data, ref index, endOffset, skipProperties, skipParse, nameTable);

      if (skipParse) return;

      ObjectTableReference = BitConverter.ToInt32(data, index); index += sizeof(int);
    }

    #endregion Overrides

  }

}
