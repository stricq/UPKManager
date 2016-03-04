using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public HexController(HexViewModel ViewModel, IMessenger Messenger) {
      viewModel = ViewModel;

      viewModel.Title   = "No Display";
      viewModel.HexData = new ObservableCollection<DomainHexData>();

      messenger = Messenger;

      registerMessages();
      registerCommands();
    }

    #endregion Constructor

    #region Messages

    private void registerMessages() {
      messenger.Register<FileLoadingMessage>(this, onFileLoadingMessage);

      messenger.RegisterAsync<ExportTableEntrySelectedMessage>(this, onExportTableEntrySelectedMessage);

      messenger.Register<ApplicationClosingMessage>(this, onApplicationClosing);
    }

    private void onFileLoadingMessage(FileLoadingMessage message) {
      tokenSource?.Cancel();

      viewModel.HexData.Clear();

      viewModel.Title = "No Display";
    }

    private async Task onExportTableEntrySelectedMessage(ExportTableEntrySelectedMessage message) {
      tokenSource?.Cancel();

      tokenSource = new CancellationTokenSource();

      viewModel.Title = message.ExportTableEntry.NameIndex.Name;

      title = viewModel.Title;

      if (message.ExportTableEntry.DomainObject != null) {
        await buildHexDataAsync(message.ExportTableEntry.DomainObject.AdditionalDataReader.GetByteArray(), message.ExportTableEntry.DomainObject.AdditionalDataOffset, tokenSource.Token);
      }
      else {
        await buildHexDataAsync(message.ExportTableEntry.DomainObjectReader.GetByteArray(), message.ExportTableEntry.SerialDataOffset, tokenSource.Token);
      }
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

    private async Task buildHexDataAsync(byte[] data, int fileOffset, CancellationToken token) {
      viewModel.HexData.Clear();

      List<DomainHexData> cache = new List<DomainHexData>();

      for(int i = 0; i < data.Length; i += 16) {
        if (token.IsCancellationRequested) return;

        StringBuilder hexStr = new StringBuilder();
        StringBuilder ascStr = new StringBuilder();

        int innerI = i;

        await Task.Run(() => {
          for(int j = 0; j < 16; ++j) {
            if (innerI + j >= data.Length || token.IsCancellationRequested) break;

            byte c = data[innerI + j];

            hexStr.AppendFormat("{0:X2} ", c);
            ascStr.Append(isDisplayable(c) ? (char)c : '.');
          }
        }, token);

        if (token.IsCancellationRequested) return;

        DomainHexData hexData = new DomainHexData {
          FileIndex   = fileOffset + i,
          Index       = i,
          HexValues   = hexStr.ToString().Trim(),
          AsciiValues = ascStr.ToString()
        };

        cache.Add(hexData);

        if (cache.Count > 48) {
          viewModel.HexData.AddRange(cache);

          viewModel.Title = $"{title} {String.Join("", Enumerable.Repeat(".", DateTime.Now.Second % 4))}";

          cache.Clear();
        }

      }

      if (cache.Any()) viewModel.HexData.AddRange(cache);

      viewModel.Title = $"{title} \u2713";
    }

    private static bool isDisplayable(byte c) {
      return !(c < 32 || (c >= 127 && c <= 186));
    }

    #endregion Private Methods

  }

}
