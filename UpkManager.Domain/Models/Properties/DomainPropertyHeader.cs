using System.Collections.Generic;
using System.Threading.Tasks;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Helpers;


namespace UpkManager.Domain.Models.Properties {

  public class DomainPropertyHeader {

    #region Constructor

    public DomainPropertyHeader() {
      Properties = new List<DomainProperty>();
    }

    #endregion Constructor

    #region Properties

    public int TypeIndex { get; set; }

    public List<DomainProperty> Properties { get; set; }

    #endregion Properties

    #region Domain Methods

    public async Task ReadPropertyHeader(ByteArrayReader reader, DomainHeader header) {
      TypeIndex = reader.ReadInt32();

      do {
        DomainProperty property = new DomainProperty();

        await property.ReadProperty(reader, header);

        Properties.Add(property);

        if (property.NameIndex.Name == ObjectType.None.ToString()) break;
      }
      while(true);
    }

    #endregion Domain Methods

  }

}
