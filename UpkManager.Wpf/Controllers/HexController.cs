using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using STR.Common.Contracts;
using STR.Common.Extensions;
using STR.Common.Messages;

using STR.MvvmCommon;
using STR.MvvmCommon.Contracts;

using UpkManager.Wpf.Messages.FileListing;
using UpkManager.Wpf.Messages.Tables;
using UpkManager.Wpf.ViewEntities;
using UpkManager.Wpf.ViewModels;


namespace UpkManager.Wpf.Controllers {

  [Export(typeof(IController))]
  public class HexController : IController {

    #region Private Fields

    private string title;

    private CancellationTokenSource tokenSource;

    private readonly HexViewModel viewModel;

    private readonly IMessenger messenger;

    private readonly IAsyncService asyncService;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public HexController(HexViewModel ViewModel, IMessenger Messenger, IAsyncService AsyncService) {
      viewModel = ViewModel;

      viewModel.Title   = "No Display";
      viewModel.HexData = new ObservableCollection<DomainHexData>();

      messenger = Messenger;

      asyncService = AsyncService;

      registerMessages();
      registerCommands();
    }

    #endregion Constructor

    #region Messages

    private void registerMessages() {
      messenger.Register<FileLoadingMessage>(this, onFileLoading);

      messenger.Register<FileLoadedMessage>(this, onFileLoaded);

      messenger.Register<ExportTableEntrySelectedMessage>(this, onExportTableEntrySelected);

      messenger.Register<PropertySelectedMessage>(this, onPropertySelected);

      messenger.Register<ApplicationClosingMessage>(this, onApplicationClosing);
    }

    private void onFileLoading(FileLoadingMessage message) {
      tokenSource?.Cancel();

      viewModel.HexData.Clear();

      viewModel.Title = "No Display";
    }

    private void onFileLoaded(FileLoadedMessage message) {
      viewModel.Title = "Depends Table";

      title = viewModel.Title;

      Task.Run(() => buildHexDataAsync(message.File.Header.DependsTable, message.File.Header.DependsTableOffset, resetToken())).FireAndForget();
    }

    private void onExportTableEntrySelected(ExportTableEntrySelectedMessage message) {
      viewModel.Title = message.ExportTableEntry.NameTableIndex.Name;

      title = viewModel.Title;

      if (message.ExportTableEntry.DomainObject?.AdditionalDataReader != null) {
        Task.Run(() => buildHexDataAsync(message.ExportTableEntry.DomainObject.AdditionalDataReader.GetBytes(), message.ExportTableEntry.DomainObject.AdditionalDataOffset, resetToken())).FireAndForget();
      }
      else {
        Task.Run(() => buildHexDataAsync(message.ExportTableEntry.DomainObjectReader.GetBytes(), message.ExportTableEntry.SerialDataOffset, resetToken())).FireAndForget();
      }
    }

    private void onPropertySelected(PropertySelectedMessage message) {
      byte[] data = message.Property.Value?.PropertyValue as byte[];

      if (data == null) return;

      viewModel.Title = message.Property.NameIndex.Name;

      title = viewModel.Title;

      Task.Run(() => buildHexDataAsync(data, 0, resetToken())).FireAndForget();
    }

    private void onApplicationClosing(ApplicationClosingMessage message) {
      tokenSource?.Cancel();
    }

    #endregion Messages

    #region Commands

    private void registerCommands() {
      viewModel.SearchBackward = new RelayCommand(onSearchBackwardExecute, canSearchBackwardExecute);
      viewModel.SearchForward  = new RelayCommand(onSearchForwardExecute, canSearchForwardExecute);
    }

    #region SearchBackward Command

    private bool canSearchBackwardExecute() {
      return viewModel.HexData.Any();
    }

    private void onSearchBackwardExecute() {

    }

    #endregion SearchBackward Command

    #region SearchForward Command

    private bool canSearchForwardExecute() {
      return viewModel.HexData.Any();
    }

    private void onSearchForwardExecute() {

    }

    #endregion SearchForward Command

    #endregion Commands

    #region Private Methods

    private CancellationToken resetToken() {
      tokenSource?.Cancel();

      tokenSource = new CancellationTokenSource();

      return tokenSource.Token;
    }

    private void buildHexDataAsync(byte[] data, int fileOffset, CancellationToken token) {
      try {
        asyncService.RunUiContext(token, () => viewModel.HexData.Clear()).Wait(token);

        List<DomainHexData> cache = new List<DomainHexData>();

        for(int i = 0; i < data.Length; i += 16) {
          if (token.IsCancellationRequested) return;

          StringBuilder hexStr = new StringBuilder();
          StringBuilder ascStr = new StringBuilder();

          for(int j = 0; j < 16; ++j) {
            if (i + j >= data.Length || token.IsCancellationRequested) break;

            byte c = data[i + j];

            hexStr.AppendFormat("{0:X2} ", c);
            ascStr.Append(isDisplayable(c) ? (char)c : '.');
          }

          if (token.IsCancellationRequested) return;

          DomainHexData hexData = new DomainHexData {
            FileIndex   = fileOffset + i,
            Index       = i,
            HexValues   = hexStr.ToString().Trim(),
            AsciiValues = ascStr.ToString()
          };

          cache.Add(hexData);

          if (cache.Count > 512) {
            int i1 = i;

            asyncService.RunUiContext(token, () => {
              viewModel.HexData.AddRange(cache);

              viewModel.Title = $"{title} {i1 / (double)data.Length:P1}";
            }).Wait(token);

            cache.Clear();
          }

        }

        asyncService.RunUiContext(token, () => {
          if (cache.Any()) viewModel.HexData.AddRange(cache);

          viewModel.Title = $"{title} \u2713";
        }).Wait(token);
      }
      catch(TaskCanceledException) { }
      catch(Exception ex) {
        messenger.SendUi(new ApplicationErrorMessage { HeaderText = "Error Building Hex Display", Exception = ex });
      }
    }

    private static bool isDisplayable(byte c) {
      return !(c < 32 || (c >= 127 && c <= 186));
    }

    #endregion Private Methods

  }

}
