using System.ComponentModel.Composition;

using AutoMapper;

using UpkManager.Domain.Contracts;
using UpkManager.Domain.Models;

using UpkManager.Entities;


namespace UpkManager.Repository.Mapping {

  [Export(typeof(IAutoMapperConfiguration))]
  public class AutoMapperConfiguration : IAutoMapperConfiguration {

    #region IAutoMapperConfiguration Implementation

    public void RegisterMappings(IMapperConfiguration config) {

      #region DTOs

      config.CreateMap<UpkFile, DomainUpkFile>().ForMember(dest => dest.IsErrored,            opt => opt.Ignore())
                                                .ForMember(dest => dest.ContainsTargetObject, opt => opt.Ignore())
                                                .ForMember(dest => dest.Filename,             opt => opt.Ignore())
                                                .ForMember(dest => dest.ModdedFiles,          opt => opt.Ignore())
                                                .ForMember(dest => dest.IsModded,             opt => opt.Ignore())
                                                .ForMember(dest => dest.Header,               opt => opt.Ignore())
                                                .ReverseMap();

      #endregion DTOs

    }

    #endregion IAutoMapperConfiguration Implementation

  }

}
