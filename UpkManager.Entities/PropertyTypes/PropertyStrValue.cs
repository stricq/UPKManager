using UpkManager.Entities.Constants;


namespace UpkManager.Entities.PropertyTypes {

  public class PropertyStrValue : PropertyValueBase {

    #region Private Fields

    private new UpkString data;

    #endregion Private Fields

    #region Overrides

    public override PropertyType PropertyType => PropertyType.StrProperty;

    public override object Value {
      get { return data; }
      set { data = (UpkString)value; }
    }

    public override void ReadPropertyValue(byte[] Data, ref int Index, UpkHeader header, out string message) {
      message = null;

      data = new UpkString();

      data.ReadUpkStr(Data, ref Index);
    }

    public override string ToString() {
      return $"{data.String}";
    }

    #endregion Overrides

  }

}
