using System.ComponentModel.Composition;
using System.IO;

using CSharpImageLibrary.General;

using STR.MvvmCommon.Contracts;

using UpkManager.Domain.Contracts;
using UpkManager.Wpf.Messages.FileListing;
using UpkManager.Wpf.Messages.Tables;
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
      messenger.Register<ExportTableEntrySelectedMessage>(this, onExportObjectSelected);

      messenger.Register<FileLoadingMessage>(this, onFileLoading);
    }

    private void onExportObjectSelected(ExportTableEntrySelectedMessage message) {
      if (message.ExportTableEntry.DomainObject != null && message.ExportTableEntry.DomainObject.IsViewable) {
        Stream stream = message.ExportTableEntry.DomainObject.GetObjectStream();

        if (stream != null) {
          ImageEngineImage image = new ImageEngineImage(stream);

          viewModel.Texture = image.GetWPFBitmap();

          stream.Close();

          return;
        }
      }

      viewModel.Texture = null;
    }

    private void onFileLoading(FileLoadingMessage message) {
      viewModel.Texture = null;
    }

    #endregion Messages

  }

}
