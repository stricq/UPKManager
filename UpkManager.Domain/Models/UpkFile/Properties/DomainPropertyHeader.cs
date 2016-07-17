using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Helpers;


namespace UpkManager.Domain.Models.UpkFile.Properties {

  public sealed class DomainPropertyHeader : DomainUpkBuilderBase {

    #region Constructor

    internal DomainPropertyHeader() {
      Properties = new List<DomainProperty>();
    }

    #endregion Constructor

    #region Properties

    public int TypeIndex { get; private set; }

    public List<DomainProperty> Properties { get; }

    #endregion Properties

    #region Domain Methods

    internal async Task ReadPropertyHeader(ByteArrayReader reader, DomainHeader header) {
      TypeIndex = reader.ReadInt32();

      do {
        DomainProperty property = new DomainProperty();

        await property.ReadProperty(reader, header);

        Properties.Add(property);

        if (property.NameIndex.Name == ObjectTypes.None.ToString()) break;
      }
      while(true);
    }

    internal List<DomainProperty> GetProperty(string name) {
      return Properties.Where(p => p.NameIndex.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)).ToList();
    }

    #endregion Domain Methods

    #region DomainUpkBuilderBase Implementation

    public override int GetBuilderSize() {
      BuilderSize = sizeof(int)
                  + Properties.Sum(p => p.GetBuilderSize());

      return BuilderSize;
    }

    public override async Task WriteBuffer(ByteArrayWriter Writer, int CurrentOffset) {
      Writer.WriteInt32(TypeIndex);

      foreach(DomainProperty property in Properties) await property.WriteBuffer(Writer, CurrentOffset);
    }

    #endregion DomainUpkBuilderBase Implementation

  }

}
