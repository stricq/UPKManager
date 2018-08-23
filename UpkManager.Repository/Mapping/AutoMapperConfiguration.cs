using System.ComponentModel.Composition;
using System.Linq;

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
                                                .ForMember(dest => dest.CurrentLocale,  opt => opt.Ignore())
                                                .ForMember(dest => dest.Filesize,       opt => opt.Ignore())
                                                .ForMember(dest => dest.Filehash,       opt => opt.Ignore())
                                                .ForMember(dest => dest.NewFilehash,    opt => opt.Ignore())
                                                .ForMember(dest => dest.NewLocale,      opt => opt.Ignore())
                                                .ReverseMap();

      config.CreateMap<ExportVersion, DomainExportVersion>().ForMember(dest => dest.Versions, opt => opt.ResolveUsing(src => src.Versions.Select(v => new DomainVersion(v))))
                                                            .ReverseMap()
                                                            .ForMember(dest => dest.Versions, opt => opt.ResolveUsing(src => src.Versions.Select(v => v.Version)));

      config.CreateMap<ExportType, DomainExportType>().ReverseMap();

      config.CreateMap<DomainUpkManagerException, UpkManagerException>().ForMember(dest => dest.Message,    opt => opt.MapFrom(src => src.Exception.Message))
                                                                        .ForMember(dest => dest.StackTrace, opt => opt.MapFrom(src => src.Exception.StackTrace))
                                                                        .ForMember(dest => dest.Id,         opt => opt.Ignore());

      config.CreateMap<UpkFile, UpkFile>().ForMember(dest => dest.Id, opt => opt.Ignore());

      config.CreateMap<UpkManagerException, UpkManagerException>().ForMember(dest => dest.Id, opt => opt.Ignore());

      #endregion DTOs

    }

    #endregion IAutoMapperConfiguration Implementation

  }

}
