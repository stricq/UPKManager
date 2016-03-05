using System.Threading.Tasks;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Helpers;
using UpkManager.Domain.Models.Tables;


namespace UpkManager.Domain.Models.Properties {

  public class DomainPropertyObjectValue : DomainPropertyIntValue {

    #region Properties

    public override PropertyType PropertyType => PropertyType.ObjectProperty;

    public override string PropertyString => $"0x{IntValue:X8}";

    #endregion Properties

  }

}
