using System;
using System.ComponentModel.Composition;
using System.Windows.Media.Imaging;

using STR.MvvmCommon;


namespace UpkManager.Wpf.ViewModels {

  [Export]
  [ViewModel("ImageViewModel")]
  public class ImageViewModel : ObservableObject {

    #region Private Fields

    private BitmapSource texture = new BitmapImage(new Uri("pack://application:,,,/UpkManager.Wpf;component/Images/UpkManagerShield.png"));

    #endregion Private Fields

    #region Properties

    public BitmapSource Texture {
      get { return texture; }
      set { SetField(ref texture, value, () => Texture); }
    }

    #endregion Properties

  }

}
