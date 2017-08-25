using System.ComponentModel.Composition;

using AutoMapper;

using UpkManager.Domain.Contracts;
using UpkManager.Domain.Models;

using UpkManager.Entities;


namespace UpkManager.Repository.Mapping {

  [Export(typeof(IAutoMapperConfiguration))]
  public sealed class AutoMapperConfiguration : IAutoMapperConfiguration {

    #region IAutoMapperConfiguration Implementation

    public void RegisterMappings(IMapperConfigurationExpression config) {

      #region DTOs

      config.CreateMap<UpkFile, DomainUpkFile>().ForMember(dest => dest.Filename,       opt => opt.Ignore())
                                                .ForMember(dest => dest.ModdedFiles,    opt => opt.Ignore())
                                                .ForMember(dest => dest.IsModded,       opt => opt.Ignore())
                                                .ForMember(dest => dest.Header,         opt => opt.Ignore())
                                                .ForMember(dest => dest.LastAccess,     opt => opt.Ignore())
                                                .ForMember(dest => dest.GameFilename,   opt => opt.Ignore())
                                                .ForMember(dest => dest.CurrentVersion, opt => opt.Ignore())
                                                .ReverseMap();

      config.CreateMap<ExportVersion, DomainExportVersion>().ForMember(dest => dest.Version, opt => opt.ResolveUsing(src => new DomainVersion(src.Version)))
                                                            .ReverseMap()
                                                            .ForMember(dest => dest.Version, opt => opt.MapFrom(src => src.Version.Version));

      config.CreateMap<ExportType, DomainExportType>().ReverseMap();

      config.CreateMap<DomainUpkManagerException, UpkManagerException>().ForMember(dest => dest.Message,    opt => opt.MapFrom(src => src.Exception.Message))
                                                                        .ForMember(dest => dest.StackTrace, opt => opt.MapFrom(src => src.Exception.StackTrace))
                                                                        .ForMember(dest => dest.Id,         opt => opt.Ignore());

      #endregion DTOs

    }

    #endregion IAutoMapperConfiguration Implementation

  }

}
