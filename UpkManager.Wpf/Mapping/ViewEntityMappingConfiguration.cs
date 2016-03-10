using System;
using System.ComponentModel.Composition;
using System.Windows;

using AutoMapper;

using UpkManager.Domain.Contracts;
using UpkManager.Domain.Models;
using UpkManager.Domain.Models.Compression;
using UpkManager.Domain.Models.Properties;
using UpkManager.Domain.Models.Tables;

using UpkManager.Wpf.Messages.Status;
using UpkManager.Wpf.ViewEntities;
using UpkManager.Wpf.ViewEntities.Tables;


namespace UpkManager.Wpf.Mapping {

  [Export(typeof(IAutoMapperConfiguration))]
  public class ViewEntityMappingConfiguration : IAutoMapperConfiguration {

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

      #region Messages

      config.CreateMap<DomainLoadProgress, LoadProgressMessage>().ForMember(dest => dest.CanAsync, opt => opt.Ignore());

      #endregion Messages

      #region Header

      config.CreateMap<DomainHeader, HeaderViewEntity>().ForMember(dest => dest.Group, opt => opt.MapFrom(src => src.Group.String))
                                                        .ForMember(dest => dest.Guid,  opt => opt.ResolveUsing(src => new Guid(src.Guid)));

      #endregion Header

      #region Tables

      config.CreateMap<DomainExportTableEntry, ExportTableEntryViewEntity>().ForMember(dest => dest.Guid,          opt => opt.ResolveUsing(src => new Guid(src.Guid)))
                                                                            .ForMember(dest => dest.ArchetypeName, opt => opt.MapFrom(src => src.ArchetypeReferenceNameIndex.Name))
                                                                            .ForMember(dest => dest.OwnerName,     opt => opt.MapFrom(src => src.OwnerReferenceNameIndex.Name))
                                                                            .ForMember(dest => dest.TypeName,      opt => opt.MapFrom(src => src.TypeReferenceNameIndex.Name))
                                                                            .ForMember(dest => dest.Name,          opt => opt.MapFrom(src => src.NameIndex.Name))
                                                                            .ForMember(dest => dest.IsSelected,    opt => opt.Ignore());

      config.CreateMap<DomainImportTableEntry, ImportTableEntryViewEntity>().ForMember(dest => dest.PackageName,        opt => opt.MapFrom(src => src.PackageNameIndex.Name))
                                                                            .ForMember(dest => dest.TypeName,           opt => opt.MapFrom(src => src.TypeNameIndex.Name))
                                                                            .ForMember(dest => dest.Name,               opt => opt.MapFrom(src => src.NameIndex.Name))
                                                                            .ForMember(dest => dest.OwnerReferenceName, opt => opt.MapFrom(src => src.OwnerReferenceNameIndex.Name))
                                                                            .ForMember(dest => dest.IsErrored,          opt => opt.Ignore())
                                                                            .ForMember(dest => dest.IsSelected,         opt => opt.Ignore());

      config.CreateMap<DomainNameTableEntry, NameTableEntryViewEntity>().ForMember(dest => dest.Name,       opt => opt.MapFrom(src => src.Name.String))
                                                                        .ForMember(dest => dest.IsErrored,  opt => opt.Ignore())
                                                                        .ForMember(dest => dest.IsSelected, opt => opt.Ignore());

      config.CreateMap<DomainGenerationTableEntry, GenerationsTableEntryViewEntity>().ForMember(dest => dest.IsErrored,  opt => opt.Ignore())
                                                                                     .ForMember(dest => dest.IsSelected, opt => opt.Ignore());

      config.CreateMap<DomainCompressedChunk, CompressionTableEntryViewEntity>().ForMember(dest => dest.BlockSize,         opt => opt.MapFrom(src => src.Header.BlockSize))
                                                                                .ForMember(dest => dest.CompressionBlocks, opt => opt.MapFrom(src => src.Header.Blocks))
                                                                                .ForMember(dest => dest.IsErrored,         opt => opt.Ignore())
                                                                                .ForMember(dest => dest.IsSelected,        opt => opt.Ignore());

      config.CreateMap<DomainCompressedChunkBlock, CompressionBlockViewEntity>().ForMember(dest => dest.IsSelected, opt => opt.Ignore());

      #endregion Tables

      #region Properties

      config.CreateMap<DomainProperty, PropertyViewEntity>().ForMember(dest => dest.Name,          opt => opt.MapFrom(src => src.NameIndex.Name))
                                                            .ForMember(dest => dest.TypeName,      opt => opt.MapFrom(src => src.TypeNameIndex.Name))
                                                            .ForMember(dest => dest.PropertyValue, opt => opt.MapFrom(src => src.Value.PropertyString))
                                                            .ForMember(dest => dest.IsErrored,     opt => opt.Ignore())
                                                            .ForMember(dest => dest.IsSelected,    opt => opt.Ignore());

      #endregion Properties

      #region DTOs

      config.CreateMap<DomainUpkFile, FileViewEntity>().ForMember(dest => dest.IsChecked,  opt => opt.Ignore())
                                                       .ForMember(dest => dest.IsSelected, opt => opt.Ignore())
                                                       .ForMember(dest => dest.IsErrored,  opt => opt.Ignore());

      #endregion DTOs

    }

    #endregion IAutoMapperConfiguration Implementation

  }

}
