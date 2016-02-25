using System.ComponentModel.Composition;

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

      config.CreateMap<DomainSettings, SettingsViewEntity>().ReverseMap();

      #endregion Settings

    }

    #endregion IAutoMapperConfiguration Implementation

  }

}
