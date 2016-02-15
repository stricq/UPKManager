using System;
using System.IO;

using UpkManager.Entities.Constants;
using UpkManager.Entities.PropertyTypes;


namespace UpkManager.Entities.ObjectTypes {

  public class ObjectBase {

    #region Properties

    public PropertyHeader PropertyHeader { get; set; }

    public int AdditionalDataOffset { get; set; }

    public byte[] AdditionalData { get; set; }

    public virtual ObjectType ObjectType => ObjectType.Unknown;

    public virtual bool CanObjectSave => false;

    #endregion Properties

    #region Public Virtual Methods

    public virtual void ReadUpkObject(byte[] data, ref int index, int endOffset, bool skipProperties, bool skipParse, UpkHeader header, out string message) {
      message = null;

      PropertyHeader = new PropertyHeader();

      if (!skipProperties) PropertyHeader.ReadPropertyHeader(data, ref index, header, out message);

      if (!String.IsNullOrEmpty(message)) return;

      int remaining = endOffset - index;

      if (remaining == 0) return;

      if (remaining < 0) {
        message = $"Offset error after parsing Properties.  Remaining bytes is negative ({remaining}).  Index is 0x{index:X8}";

        return;
      }

      AdditionalDataOffset = index;

      AdditionalData = new byte[remaining];

      Array.ConstrainedCopy(data, index, AdditionalData, 0, remaining);
    }

    public virtual void SaveObject(string filename) { }

    public virtual Stream GetObjectStream() {
      return null;
    }

    #endregion Public Methods

  }

}
