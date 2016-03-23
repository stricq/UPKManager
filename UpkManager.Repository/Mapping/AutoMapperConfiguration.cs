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

      config.CreateMap<UpkFile, DomainUpkFile>().ForMember(dest => dest.ContainsTargetObject, opt => opt.Ignore())
                                                .ForMember(dest => dest.Filename,             opt => opt.Ignore())
                                                .ForMember(dest => dest.ModdedFiles,          opt => opt.Ignore())
                                                .ForMember(dest => dest.IsModded,             opt => opt.Ignore())
                                                .ForMember(dest => dest.Header,               opt => opt.Ignore())
                                                .ForMember(dest => dest.LastAccess,           opt => opt.Ignore())
                                                .ReverseMap();

      config.CreateMap<DomainUpkManagerException, UpkManagerException>().ForMember(dest => dest.Message,    opt => opt.MapFrom(src => src.Exception.Message))
                                                                        .ForMember(dest => dest.StackTrace, opt => opt.MapFrom(src => src.Exception.StackTrace))
                                                                        .ForMember(dest => dest.Id,         opt => opt.Ignore());

      #endregion DTOs

    }

    #endregion IAutoMapperConfiguration Implementation

  }

}
