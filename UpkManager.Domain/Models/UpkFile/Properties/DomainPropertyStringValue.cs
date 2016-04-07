using System.Threading.Tasks;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Helpers;


namespace UpkManager.Domain.Models.UpkFile.Properties {

  public sealed class DomainPropertyStringValue : DomainPropertyValueBase {

    #region Constructor

    public DomainPropertyStringValue() {
      stringValue = new DomainString();
    }

    #endregion Constructor

    #region Properties

    private DomainString stringValue { get; }

    #endregion Properties

    #region Domain Properties

    public override PropertyTypes PropertyType => PropertyTypes.StrProperty;

    public override object PropertyValue => stringValue;

    public override string PropertyString => stringValue.String;

    #endregion Domain Properties

    #region Domain Methods

    public override async Task ReadPropertyValue(ByteArrayReader reader, int size, DomainHeader header) {
      await stringValue.ReadString(reader);
    }

    public override void SetPropertyValue(object value) {
      string str = value as string;

      if (str == null) return;

      stringValue.SetString(str);
    }

    #endregion Domain Methods

    #region DomainUpkBuilderBase Implementation

    public override int GetBuilderSize() {
      BuilderSize = stringValue.GetBuilderSize();

      return BuilderSize;
    }

    public override async Task WriteBuffer(ByteArrayWriter Writer, int CurrentOffset) {
      await stringValue.WriteBuffer(Writer, CurrentOffset);
    }

    #endregion DomainUpkBuilderBase Implementation

  }

}
