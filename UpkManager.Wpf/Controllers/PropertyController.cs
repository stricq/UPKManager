using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

using AutoMapper;

using Ookii.Dialogs.Wpf;

using STR.MvvmCommon;
using STR.MvvmCommon.Contracts;

using UpkManager.Domain.Contracts;
using UpkManager.Domain.Models.Tables;

using UpkManager.Wpf.Messages.FileListing;
using UpkManager.Wpf.Messages.Tables;
using UpkManager.Wpf.ViewEntities;
using UpkManager.Wpf.ViewModels;


namespace UpkManager.Wpf.Controllers {

  [Export(typeof(IController))]
  public class PropertyController : IController {

    #region Private Fields

    private DomainExportTableEntry export;

    private readonly PropertyViewModel     viewModel;
    private readonly MainMenuViewModel menuViewModel;

    private readonly IMessenger messenger;
    private readonly IMapper    mapper;

    private readonly IUpkFileRepository fileRepository;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public PropertyController(PropertyViewModel ViewModel, MainMenuViewModel MenuViewModel, IMessenger Messenger, IMapper Mapper, IUpkFileRepository FileRepository) {
          viewModel = ViewModel;
      menuViewModel = MenuViewModel;

      viewModel.Properties = new ObservableCollection<PropertyViewEntity>();

      messenger = Messenger;
         mapper = Mapper;

      fileRepository = FileRepository;

      registerMessages();
      registerCommands();
    }

    #endregion Constructor

    #region Messages

    private void registerMessages() {
      messenger.Register<ExportTableEntrySelectedMessage>(this, onExportObjectSelected);

      messenger.Register<FileLoadingMessage>(this, onFileLoading);
    }

    private void onExportObjectSelected(ExportTableEntrySelectedMessage message) {
      export = message.ExportTableEntry;

      viewModel.Properties = new ObservableCollection<PropertyViewEntity>(mapper.Map<IEnumerable<PropertyViewEntity>>(export.DomainObject.PropertyHeader.Properties));
    }

    private void onFileLoading(FileLoadingMessage message) {
      viewModel.Properties.Clear();

      export = null;
    }

    #endregion Messages

    #region Commands

    private void registerCommands() {
      menuViewModel.SaveObjectAs = new RelayCommandAsync(onSaveObjectAsExecute, canSaveObjectAsExecute);
    }

    private bool canSaveObjectAsExecute() {
      return export != null && export.DomainObject.IsExportable;
    }

    private async Task onSaveObjectAsExecute() {
      VistaSaveFileDialog sfd = new VistaSaveFileDialog {
        DefaultExt = ".dds",
        Filter = "DirectDraw Surface|*.dds",
        Title = "Save Object As..."
      };

      bool? result = sfd.ShowDialog();

      if (!result.HasValue || !result.Value) return;

      await fileRepository.SaveObject(export, sfd.FileName);
    }

    #endregion Commands

  }

}
