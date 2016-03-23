using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using CSharpImageLibrary.General;

using NAudio.Vorbis;
using NAudio.Wave;

using STR.Common.Extensions;
using STR.Common.Messages;

using STR.MvvmCommon.Contracts;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Models.Objects;

using UpkManager.Wpf.Messages.FileListing;
using UpkManager.Wpf.Messages.Tables;
using UpkManager.Wpf.ViewModels;


namespace UpkManager.Wpf.Controllers {

  [Export(typeof(IController))]
  public class ImageController : IController {

    #region Private Fields

    private CancellationTokenSource tokenSource;

    private readonly ImageViewModel viewModel;

    private readonly IMessenger messenger;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public ImageController(ImageViewModel ViewModel, IMessenger Messenger) {
      viewModel = ViewModel;

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
      if (message.ExportTableEntry.DomainObject == null) return;

      switch(message.ExportTableEntry.DomainObject.Viewable) {
        case ViewableTypes.Sound: {
          Task.Run(() => playSound(message.ExportTableEntry.DomainObject, resetToken())).FireAndForget();

          break;
        }

        case ViewableTypes.Image: {
          Stream stream = message.ExportTableEntry.DomainObject.GetObjectStream();

          if (stream != null) {
            ImageEngineImage image = new ImageEngineImage(stream);

            viewModel.Texture = image.GetWPFBitmap();

            stream.Close();
          }

          break;
        }

        default: {
          viewModel.Texture = null;

          break;
        }
      }
    }

    private void onFileLoading(FileLoadingMessage message) {
      viewModel.Texture = null;
    }

    #endregion Messages

    #region Private Methods

    private CancellationToken resetToken() {
      tokenSource?.Cancel();

      tokenSource = new CancellationTokenSource();

      return tokenSource.Token;
    }

    private void playSound(DomainObjectBase soundObject, CancellationToken token) {
      try {
        Stream stream = soundObject.GetObjectStream();

        using(VorbisWaveReader vorbisStream = new VorbisWaveReader(stream)) {
          using(WaveOutEvent waveOut = new WaveOutEvent()) {
            waveOut.Init(vorbisStream);

            waveOut.Play();

            while(waveOut.PlaybackState == PlaybackState.Playing) {
              if (token.IsCancellationRequested) break;

              Task.Delay(100, token);
            }
          }
        }
      }
      catch(TaskCanceledException) { }
      catch(OperationCanceledException) { }
      catch(Exception ex) {
        messenger.SendUi(new ApplicationErrorMessage { Exception = ex, HeaderText = "Error Playing Audio" });
      }
    }

    #endregion Private Methods

  }

}
