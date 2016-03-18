using System.Threading.Tasks;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Helpers;
using UpkManager.Domain.Models.Tables;


namespace UpkManager.Domain.Models.Properties {

  public sealed class DomainPropertyStructValue : DomainPropertyValueBase {

    #region Constructor

    public DomainPropertyStructValue() {
      StructNameIndex = new DomainNameTableIndex();
    }

    #endregion Constructor

    #region Properties

    public DomainNameTableIndex StructNameIndex { get; set; }

    #endregion Properties

    #region Domain Properties

    public override PropertyType PropertyType => PropertyType.StructProperty;

    public override string PropertyString => StructNameIndex.Name;

    #endregion Domain Properties

    #region Domain Methods

    public override async Task ReadPropertyValue(ByteArrayReader reader, int size, DomainHeader header) {
      await Task.Run(() => StructNameIndex.ReadNameTableIndex(reader, header));

      await base.ReadPropertyValue(reader, size, header);
    }

    #endregion Domain Methods

    #region DomainUpkBuilderBase Implementation

    public override int GetBuilderSize() {
      BuilderSize = StructNameIndex.GetBuilderSize()
                  + base.GetBuilderSize();

      return BuilderSize;
    }

    public override async Task WriteBuffer(ByteArrayWriter Writer) {
      await StructNameIndex.WriteBuffer(Writer);

      await base.WriteBuffer(Writer);
    }

    #endregion DomainUpkBuilderBase Implementation

  }

}
