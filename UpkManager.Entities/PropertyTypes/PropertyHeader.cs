using System;
using System.Collections.Generic;
using System.Linq;

using UpkManager.Entities.Constants;


namespace UpkManager.Entities.PropertyTypes {

  public class PropertyHeader {

    #region Properties

    public uint Index { get; set; }

    public List<Property> Properties { get; set; }

    #endregion Properties

    #region Public Methods

    public void ReadPropertyHeader(byte[] data, ref int index, UpkHeader header, out string message) {
      Index = BitConverter.ToUInt32(data, index); index += sizeof(uint);

      Properties = new List<Property>();

      do {
        Property prop = new Property();

        prop.ReadProperty(data, ref index, header, out message);

        if (!String.IsNullOrEmpty(message)) return;

        Properties.Add(prop);

        if (prop.NameIndex.Name == ObjectType.None.ToString()) break;
      } while(true);
    }

    public List<Property> GetProperty(string name) {
      return Properties.Where(p => p.NameIndex.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)).ToList();
    }

    #endregion Public Methods

  }

}
