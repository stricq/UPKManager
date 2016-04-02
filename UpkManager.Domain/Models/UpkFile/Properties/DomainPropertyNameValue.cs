using System.Threading.Tasks;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Helpers;
using UpkManager.Domain.Models.UpkFile.Tables;


namespace UpkManager.Domain.Models.UpkFile.Properties {

  public class DomainPropertyNameValue : DomainPropertyValueBase {

    #region Constructor

    public DomainPropertyNameValue() {
      NameIndexValue = new DomainNameTableIndex();
    }

    #endregion Constructor

    #region Properties

    protected DomainNameTableIndex NameIndexValue { get; set; }

    #endregion Properties

    #region Domain Properties

    public override PropertyTypes PropertyType => PropertyTypes.NameProperty;

    public override object PropertyValue => NameIndexValue;

    public override string PropertyString => NameIndexValue.Name;

    #endregion Domain Properties

    #region Domain Methods

    public override async Task ReadPropertyValue(ByteArrayReader reader, int size, DomainHeader header) {
      await Task.Run(() => NameIndexValue.ReadNameTableIndex(reader, header));
    }

    #endregion Domain Methods

    #region DomainUpkBuilderBase Implementation

    public override int GetBuilderSize() {
      BuilderSize = NameIndexValue.GetBuilderSize();

      return BuilderSize;
    }

    public override async Task WriteBuffer(ByteArrayWriter Writer, int CurrentOffset) {
      await NameIndexValue.WriteBuffer(Writer, CurrentOffset);
    }

    #endregion DomainUpkBuilderBase Implementation

  }

}
