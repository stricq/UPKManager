using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using NAudio.Vorbis;
using NAudio.Wave;

using STR.Common.Extensions;
using STR.Common.Messages;

using STR.MvvmCommon.Contracts;

using UpkManager.Dds;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Models.UpkFile.Objects.Textures;

using UpkManager.Wpf.Messages.FileListing;
using UpkManager.Wpf.Messages.Rebuild;
using UpkManager.Wpf.Messages.Tables;
using UpkManager.Wpf.ViewEntities;
using UpkManager.Wpf.ViewModels;


namespace UpkManager.Wpf.Controllers {

  [Export(typeof(IController))]
  public sealed class ImageController : IController {

    #region Private Fields

    private bool isSelf;

    private CancellationTokenSource tokenSource;

    private DomainObjectTexture2D texture;

    private readonly ImageViewModel viewModel;

    private readonly IMessenger messenger;

    private readonly IMapper mapper;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public ImageController(ImageViewModel ViewModel, IMessenger Messenger, IMapper Mapper) {
      viewModel = ViewModel;

      messenger = Messenger;

      mapper = Mapper;
    }

    #endregion Constructor

    #region IController Implementation

    public async Task InitializeAsync() {
      registerMessages();

      await Task.CompletedTask;
    }

    public int InitializePriority { get; } = 100;

    #endregion IController Implementation

    #region Messages

    private void registerMessages() {
      messenger.Register<ExportTableEntrySelectedMessage>(this, onExportObjectSelected);

      messenger.Register<ExportedObjectSelectedMessage>(this, onExportedObjectSelected);

      messenger.Register<FileLoadingMessage>(this, onFileLoading);

      messenger.Register<ApplicationClosingMessage>(this, onApplicationClosing);
    }

    private void onExportObjectSelected(ExportTableEntrySelectedMessage message) {
      if (message.ExportTableEntry.DomainObject == null) return;

      switch(message.ExportTableEntry.DomainObject.Viewable) {
        case ViewableTypes.Sound: {
          Task.Run(() => playSound(message.ExportTableEntry.DomainObject.GetObjectStream(), resetToken())).FireAndForget();

          break;
        }
        case ViewableTypes.Image: {
          texture = message.ExportTableEntry.DomainObject as DomainObjectTexture2D;

          if (texture != null) {
            clearViewModel();

            viewModel.MipMaps = new ObservableCollection<MipMapViewEntity>(mapper.Map<IEnumerable<MipMapViewEntity>>(texture.MipMaps));

            for(int i = 0; i < viewModel.MipMaps.Count; ++i) {
              viewModel.MipMaps[i].Level = i + 1;

              viewModel.MipMaps[i].PropertyChanged += onMipMapViewEntityChanged;
            }

            MipMapViewEntity largest = viewModel.MipMaps.Aggregate((i1, i2) => i1 != null && i1.IsEnabled & (i1.Width > i2.Width || i1.Height > i2.Height) ? i1 : (i2.IsEnabled ? i2 : null));

            if (largest != null) largest.IsChecked = true;
          }

          break;
        }
        default: {
          clearViewModel();

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
          DdsFile image = new DdsFile(message.Filename);

          viewModel.MipMaps = null;

          viewModel.Texture = image.BitmapSource;

          break;
        }
      }
    }

    private void onFileLoading(FileLoadingMessage message) {
      clearViewModel();
    }

    private void onApplicationClosing(ApplicationClosingMessage message) {
      tokenSource?.Cancel();
    }

    #endregion Messages

    #region Private Methods

    private void clearViewModel() {
      viewModel.MipMaps?.ForEach(mip => mip.PropertyChanged -= onMipMapViewEntityChanged);

      viewModel.MipMaps = null;
      viewModel.Texture = null;
    }

    private void onMipMapViewEntityChanged(object sender, PropertyChangedEventArgs args) {
      if (isSelf) return;

      MipMapViewEntity entity = sender as MipMapViewEntity;

      if (entity == null) return;

      switch(args.PropertyName) {
        case "IsChecked": {
          if (entity.IsChecked) {
            isSelf = true;

            viewModel.MipMaps.Where(mip => mip != entity).ToList().ForEach(mip => mip.IsChecked = false);

            isSelf = false;

            Stream stream = texture.GetObjectStream(entity.Level - 1);

            if (stream != null) {
              DdsFile ddsFile = new DdsFile();

              ddsFile.Load(stream);

              viewModel.Texture = ddsFile.BitmapSource;

              stream.Close();
            }
          }
          else {
            isSelf = true;

            entity.IsChecked = true;

            isSelf = false;
          }

          break;
        }
      }
    }

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
        stream?.Close();
      }
    }

    #endregion Private Methods

  }

}
