using System.ComponentModel.Composition;
using System.Windows;

using AutoMapper;

using UpkManager.Domain.Contracts;
using UpkManager.Domain.Models;

using UpkManager.Wpf.ViewEntities;


namespace UpkManager.Wpf.Mapping {

  [Export(typeof(IAutoMapperConfiguration))]
  public class AutoMapperConfiguration : IAutoMapperConfiguration {

    #region IAutoMapperConfiguration Implementation

    public void RegisterMappings(IMapperConfiguration config) {

      #region Settings

      config.CreateMap<DomainSettings, SettingsDialogViewEntity>().ReverseMap();

      config.CreateMap<DomainSettings, SettingsWindowViewEntity>().ForMember(dest => dest.AreSettingsChanged, opt => opt.Ignore())
                                                                  .ForMember(dest => dest.MainWindowState,    opt => opt.ResolveUsing(src => src.Maximized ? WindowState.Maximized : WindowState.Normal));

      config.CreateMap<SettingsWindowViewEntity, DomainSettings>().ForMember(dest => dest.Maximized,  opt => opt.ResolveUsing(src => src.MainWindowState == WindowState.Maximized))
                                                                  .ForMember(dest => dest.PathToGame, opt => opt.Ignore())
                                                                  .ForMember(dest => dest.ExportPath, opt => opt.Ignore());

      #endregion Settings

    }

    #endregion IAutoMapperConfiguration Implementation

  }

}
