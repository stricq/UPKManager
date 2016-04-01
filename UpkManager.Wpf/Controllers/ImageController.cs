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

using UpkManager.Wpf.Messages.FileListing;
using UpkManager.Wpf.Messages.Rebuild;
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

      messenger.Register<ExportedObjectSelectedMessage>(this, onExportedObjectSelected);

      messenger.Register<FileLoadingMessage>(this, onFileLoading);
    }

    private void onExportObjectSelected(ExportTableEntrySelectedMessage message) {
      if (message.ExportTableEntry.DomainObject == null) return;

      switch(message.ExportTableEntry.DomainObject.Viewable) {
        case ViewableTypes.Sound: {
          Task.Run(() => playSound(message.ExportTableEntry.DomainObject.GetObjectStream(), resetToken())).FireAndForget();

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

    private void onExportedObjectSelected(ExportedObjectSelectedMessage message) {
      string extension = Path.GetExtension(message.Filename)?.ToLowerInvariant();

      switch(extension) {
        case ".ogg": {
          Task.Run(() => playSound(new FileStream(message.Filename, FileMode.Open), resetToken())).FireAndForget();

          break;
        }
        case ".dds": {
          ImageEngineImage image = new ImageEngineImage(message.Filename);

          viewModel.Texture = image.GetWPFBitmap();

          break;
        }
        default: {
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

    private void playSound(Stream stream, CancellationToken token) {
      try {
        using(VorbisWaveReader vorbisStream = new VorbisWaveReader(stream)) {
          using(WaveOutEvent waveOut = new WaveOutEvent()) {
            waveOut.Init(vorbisStream);

#pragma warning disable 612
            waveOut.Volume = 0.7f; // If only there were an example of setting volume the correct way....
#pragma warning restore 612

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
      finally {
        stream.Close();
      }
    }

    #endregion Private Methods

  }

}
