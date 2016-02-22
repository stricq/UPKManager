using System.ComponentModel.Composition;
using System.IO;

using CSharpImageLibrary.General;

using STR.MvvmCommon.Contracts;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Contracts;
using UpkManager.Domain.Messages.FileHeader;

using UpkManager.Wpf.ViewModels;


namespace UpkManager.Wpf.Controllers {

  [Export(typeof(IController))]
  public class ImageController : IController {

    #region Private Fields

    private readonly ImageViewModel viewModel;

    private readonly IMessenger messenger;

    private readonly IUpkFileRepository repository;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public ImageController(ImageViewModel ViewModel, IMessenger Messenger, IUpkFileRepository Repository) {
      viewModel = ViewModel;

      repository = Repository;
      messenger  = Messenger;

      registerMessages();
    }

    #endregion Constructor

    #region Messages

    private void registerMessages() {
      messenger.Register<ExportObjectSelectedMessage>(this, onExportObjectSelected);

      messenger.Register<FileHeaderLoadingMessage>(this, onFileHeaderLoading);
    }

    private void onExportObjectSelected(ExportObjectSelectedMessage message) {
      if (message.ExportObject.DomainObject.ObjectType == ObjectType.Texture2D) {
        Stream stream = repository.GetObjectStream(message.ExportObject);

        if (stream != null) {
          ImageEngineImage image = new ImageEngineImage(stream);

          viewModel.Texture = image.GetWPFBitmap();

          stream.Close();

          return;
        }
      }

      viewModel.Texture = null;
    }

    private void onFileHeaderLoading(FileHeaderLoadingMessage message) {
      viewModel.Texture = null;
    }

    #endregion Messages

  }

}
