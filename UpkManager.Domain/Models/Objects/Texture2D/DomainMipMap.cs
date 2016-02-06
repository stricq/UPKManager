using STR.MvvmCommon;


namespace UpkManager.Domain.Models.Objects.Texture2D {

  public class DomainMipMap : ObservableObject {

    #region Private Fields

    private int width;
    private int height;

    private byte[] imageData;

    #endregion Private Fields

    #region Properties

    public int Width {
      get { return width; }
      set { SetField(ref width, value, () => Width); }
    }

    public int Height {
      get { return height; }
      set { SetField(ref height, value, () => Height); }
    }

    public byte[] ImageData {
      get { return imageData; }
      set { SetField(ref imageData, value, () => ImageData); }
    }

    #endregion Properties

  }

}
