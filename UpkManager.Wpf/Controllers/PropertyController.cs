using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Ookii.Dialogs.Wpf;

using STR.Common.Extensions;

using STR.MvvmCommon;
using STR.MvvmCommon.Contracts;

using UpkManager.Dds;
using UpkManager.Dds.Constants;

using UpkManager.Domain.Models.UpkFile.Properties;
using UpkManager.Domain.Models.UpkFile.Tables;

using UpkManager.Wpf.Messages.FileListing;
using UpkManager.Wpf.Messages.Tables;
using UpkManager.Wpf.ViewEntities;
using UpkManager.Wpf.ViewModels;


namespace UpkManager.Wpf.Controllers {

  [Export(typeof(IController))]
  public sealed class PropertyController : IController {

    #region Private Fields

    private DomainExportTableEntry export;

    private readonly PropertyViewModel     viewModel;
    private readonly MainMenuViewModel menuViewModel;

    private readonly IMessenger messenger;
    private readonly IMapper    mapper;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public PropertyController(PropertyViewModel ViewModel, MainMenuViewModel MenuViewModel, IMessenger Messenger, IMapper Mapper) {
          viewModel = ViewModel;
      menuViewModel = MenuViewModel;

      viewModel.Properties = new ObservableCollection<PropertyViewEntity>();

      messenger = Messenger;
         mapper = Mapper;
    }

    #endregion Constructor

    #region IController Implementation

    public async Task InitializeAsync() {
      registerMessages();
      registerCommands();

      await Task.CompletedTask;
    }

    public int InitializePriority { get; } = 100;

    #endregion IController Implementation

    #region Messages

    private void registerMessages() {
      messenger.Register<FileLoadingMessage>(this, onFileLoading);

      messenger.Register<ExportTableEntrySelectedMessage>(this, onExportObjectSelected);
    }

    private void onFileLoading(FileLoadingMessage message) {
      viewModel.Properties.ForEach(p => p.PropertyChanged -= onPropertyChanged);

      viewModel.Properties.Clear();

      export = null;
    }

    private void onExportObjectSelected(ExportTableEntrySelectedMessage message) {
      export = message.ExportTableEntry;

      viewModel.Properties.ForEach(p => p.PropertyChanged -= onPropertyChanged);

      viewModel.Properties = new ObservableCollection<PropertyViewEntity>(mapper.Map<IEnumerable<PropertyViewEntity>>(export.DomainObject.PropertyHeader.Properties));

      viewModel.Properties.ForEach(p => p.PropertyChanged += onPropertyChanged);
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
        FileName   = export.NameTableIndex.Name,
        DefaultExt = export.DomainObject.FileExtension,
        Filter     = $"{export.DomainObject.FileTypeDesc}|*{export.DomainObject.FileExtension}",
        Title      = "Save Object As..."
      };

      bool? result = sfd.ShowDialog();

      if (!result.HasValue || !result.Value) return;

      int compressor = menuViewModel.IsCompressorClusterFit ? 0 : menuViewModel.IsCompressorRangeFit ? 1 : 2;

      int errorMetric = menuViewModel.IsErrorMetricPerceptual ? 0 : 1;

      DdsSaveConfig config = new DdsSaveConfig(FileFormat.Unknown, compressor, errorMetric, menuViewModel.IsWeightColorByAlpha, false);

      await export.DomainObject.SaveObject(sfd.FileName, config);
    }

    #endregion Commands

    #region Private Methods

    private void onPropertyChanged(object sender, PropertyChangedEventArgs args) {
      PropertyViewEntity propertyViewEntity = sender as PropertyViewEntity;

      if (propertyViewEntity == null) return;

      switch(args.PropertyName) {
        case "IsSelected": {
          if (propertyViewEntity.IsSelected) {
            DomainProperty property = export.DomainObject.PropertyHeader.Properties.FirstOrDefault(p => p.NameIndex.Name == propertyViewEntity.Name
                                                                                                     && p.ArrayIndex     == propertyViewEntity.ArrayIndex);

            if (property == null) return;

            messenger.Send(new PropertySelectedMessage { Property = property });
          }

          break;
        }
        default: {
          break;
        }
      }
    }

    #endregion Private Methods

  }

}
