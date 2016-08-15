using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Media.Imaging;

using STR.MvvmCommon;

using UpkManager.Wpf.ViewEntities;


namespace UpkManager.Wpf.ViewModels {

  [Export]
  [ViewModel("ImageViewModel")]
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  [SuppressMessage("ReSharper", "MemberCanBeInternal")]
  public sealed class ImageViewModel : ObservableObject {

    #region Private Fields

    private BitmapSource texture = new BitmapImage(new Uri("pack://application:,,,/UpkManager.Wpf;component/Images/UpkManagerShield.png"));

    private ObservableCollection<MipMapViewEntity> mipMaps;

    #endregion Private Fields

    #region Properties

    public BitmapSource Texture {
      get { return texture; }
      set { SetField(ref texture, value, () => Texture); }
    }

    public ObservableCollection<MipMapViewEntity> MipMaps {
      get { return mipMaps; }
      set { SetField(ref mipMaps, value, () => MipMaps, () => IsLineVisible); }
    }

    public bool IsLineVisible => MipMaps != null;

    #endregion Properties

  }

}
