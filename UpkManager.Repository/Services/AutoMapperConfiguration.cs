using System.ComponentModel.Composition;

using AutoMapper;

using UpkManager.Domain.Contracts;
using UpkManager.Domain.Models;
using UpkManager.Domain.Models.Compression;
using UpkManager.Domain.Models.Objects;
using UpkManager.Domain.Models.Objects.Texture2D;
using UpkManager.Domain.Models.Properties;
using UpkManager.Domain.Models.Tables;

using UpkManager.Entities;
using UpkManager.Entities.Compression;
using UpkManager.Entities.ObjectTypes;
using UpkManager.Entities.ObjectTypes.Texture2D;
using UpkManager.Entities.PropertyTypes;
using UpkManager.Entities.Tables;


namespace UpkManager.Repository.Services {

  [Export(typeof(IAutoMapperConfiguration))]
  public class AutoMapperConfiguration : IAutoMapperConfiguration {

    #region IAutoMapperConfiguration Implementation

    public void RegisterMappings(IMapperConfiguration config) {

      #region Top Level

      config.CreateMap<UpkHeader, DomainHeader>().ForMember(dest => dest.FullFilename, opt => opt.Ignore())
                                                 .ForMember(dest => dest.Filename,     opt => opt.Ignore())
                                                 .ForMember(dest => dest.GuidString,   opt => opt.Ignore())
                                                 .ReverseMap();

      config.CreateMap<UpkString, DomainString>().ReverseMap();

      #endregion Top Level

      #region Compression

      config.CreateMap<CompressedChunk, DomainCompressedChunk>().ForMember(dest => dest.IsSelected, opt => opt.Ignore())
                                                                .ReverseMap();

      config.CreateMap<CompressedChunkHeader, DomainCompressedChunkHeader>().ForMember(dest => dest.IsSelected, opt => opt.Ignore())
                                                                            .ReverseMap();

      config.CreateMap<CompressedChunkBlock, DomainCompressedChunkBlock>().ForMember(dest => dest.IsSelected, opt => opt.Ignore())
                                                                          .ReverseMap();

      #endregion Compression

      #region Tables

      config.CreateMap<GenerationTableEntry, DomainGenerationTableEntry>().ForMember(dest => dest.IsSelected, opt => opt.Ignore())
                                                                          .ReverseMap();

      config.CreateMap<NameTableEntry, DomainNameTableEntry>().ForMember(dest => dest.IsSelected, opt => opt.Ignore())
                                                              .ForMember(dest => dest.NameString, opt => opt.Ignore())
                                                              .ReverseMap();

      config.CreateMap<NameTableIndex, DomainNameTableIndex>().ReverseMap();

      config.CreateMap<ObjectTableEntry, DomainObjectTableEntry>().Include<ImportTableEntry, DomainImportTableEntry>()
                                                                  .Include<ExportTableEntry, DomainExportTableEntry>()
                                                                  .ForMember(dest => dest.Name, opt => opt.Ignore())
                                                                  .ReverseMap();

      config.CreateMap<ImportTableEntry, DomainImportTableEntry>().ForMember(dest => dest.IsSelected,  opt => opt.Ignore())
                                                                  .ForMember(dest => dest.PackageName, opt => opt.Ignore())
                                                                  .ForMember(dest => dest.TypeName,    opt => opt.Ignore())
                                                                  .ForMember(dest => dest.Name,        opt => opt.Ignore())
                                                                  .ReverseMap();

      config.CreateMap<ExportTableEntry, DomainExportTableEntry>().ForMember(dest => dest.DomainObject, opt => opt.MapFrom(src => src.UpkObject))
                                                                  .ForMember(dest => dest.IsSelected,   opt => opt.Ignore())
                                                                  .ForMember(dest => dest.GuidString,   opt => opt.Ignore())
                                                                  .ForMember(dest => dest.TypeName,     opt => opt.Ignore())
                                                                  .ForMember(dest => dest.Name,         opt => opt.Ignore())
                                                                  .ReverseMap()
                                                                  .ForMember(dest => dest.UpkObject,    opt => opt.MapFrom(src => src.DomainObject));
      #endregion Tables

      #region Objects

      config.CreateMap<ObjectBase, DomainObjectBase>().Include<ObjectDistributionFloatUniform, DomainObjectDistributionFloatUniform>()
                                                      .Include<ObjectObjectRedirector, DomainObjectObjectRedirector>()
                                                      .Include<ObjectTexture2D, DomainObjectTexture2D>()
                                                      .ForMember(dest => dest.ObjectType, opt => opt.Ignore())
                                                      .ReverseMap()
                                                      .ForMember(dest => dest.CanObjectSave, opt => opt.Ignore());

      config.CreateMap<ObjectDistributionFloatUniform, DomainObjectDistributionFloatUniform>().ForMember(dest => dest.ObjectType, opt => opt.Ignore())
                                                                                              .ReverseMap()
                                                                                              .ForMember(dest => dest.CanObjectSave, opt => opt.Ignore());

      config.CreateMap<ObjectObjectRedirector, DomainObjectObjectRedirector>().ForMember(dest => dest.ObjectType, opt => opt.Ignore())
                                                                              .ReverseMap()
                                                                              .ForMember(dest => dest.CanObjectSave, opt => opt.Ignore());

      config.CreateMap<ObjectTexture2D, DomainObjectTexture2D>().ForMember(dest => dest.ObjectType, opt => opt.Ignore())
                                                                .ForMember(dest => dest.GuidString, opt => opt.Ignore())
                                                                .ReverseMap()
                                                                .ForMember(dest => dest.CanObjectSave, opt => opt.Ignore());

      config.CreateMap<MipMap, DomainMipMap>().ReverseMap();

      #endregion Objects

      #region Properties

      config.CreateMap<PropertyHeader, DomainPropertyHeader>().ReverseMap();

      config.CreateMap<Property, DomainProperty>().ForMember(dest => dest.Name,     opt => opt.Ignore())
                                                  .ForMember(dest => dest.TypeName, opt => opt.Ignore())
                                                  .ReverseMap();

      config.CreateMap<PropertyValueBase, DomainPropertyValueBase>().Include<PropertyBoolValue, DomainPropertyBoolValue>()
                                                                    .Include<PropertyIntValue, DomainPropertyIntValue>()
                                                                    .Include<PropertyFloatValue, DomainPropertyFloatValue>()
                                                                    .Include<PropertyObjectValue, DomainPropertyObjectValue>()
                                                                    .Include<PropertyGuidValue, DomainPropertyGuidValue>()
                                                                    .Include<PropertyNameValue, DomainPropertyNameValue>()
                                                                    .Include<PropertyStrValue, DomainPropertyStrValue>()
                                                                    .Include<PropertyStructValue, DomainPropertyStructValue>()
                                                                    .Include<PropertyArrayValue, DomainPropertyArrayValue>()
                                                                    .ForMember(dest => dest.PropertyType, opt => opt.Ignore())
                                                                    .ReverseMap();

      config.CreateMap<PropertyBoolValue, DomainPropertyBoolValue>().ForMember(dest => dest.PropertyType, opt => opt.Ignore())
                                                                    .ReverseMap()
                                                                    .ForMember(dest => dest.Size, opt => opt.Ignore());

      config.CreateMap<PropertyIntValue, DomainPropertyIntValue>().Include<PropertyObjectValue, DomainPropertyObjectValue>()
                                                                  .ForMember(dest => dest.PropertyType, opt => opt.Ignore())
                                                                  .ReverseMap()
                                                                  .ForMember(dest => dest.Size, opt => opt.Ignore());

      config.CreateMap<PropertyFloatValue, DomainPropertyFloatValue>().ForMember(dest => dest.PropertyType, opt => opt.Ignore())
                                                                      .ReverseMap()
                                                                      .ForMember(dest => dest.Size, opt => opt.Ignore());

      config.CreateMap<PropertyObjectValue, DomainPropertyObjectValue>().Include<PropertyInterfaceValue, DomainPropertyInterfaceValue>()
                                                                        .Include<PropertyComponentValue, DomainPropertyComponentValue>()
                                                                        .Include<PropertyClassValue, DomainPropertyClassValue>()
                                                                        .ForMember(dest => dest.PropertyType, opt => opt.Ignore())
                                                                        .ReverseMap()
                                                                        .ForMember(dest => dest.Size, opt => opt.Ignore());

      config.CreateMap<PropertyInterfaceValue, DomainPropertyInterfaceValue>().ForMember(dest => dest.PropertyType, opt => opt.Ignore())
                                                                              .ReverseMap()
                                                                              .ForMember(dest => dest.Size, opt => opt.Ignore());

      config.CreateMap<PropertyComponentValue, DomainPropertyComponentValue>().ForMember(dest => dest.PropertyType, opt => opt.Ignore())
                                                                              .ReverseMap()
                                                                              .ForMember(dest => dest.Size, opt => opt.Ignore());

      config.CreateMap<PropertyClassValue, DomainPropertyClassValue>().ForMember(dest => dest.PropertyType, opt => opt.Ignore())
                                                                      .ReverseMap()
                                                                      .ForMember(dest => dest.Size, opt => opt.Ignore());

      config.CreateMap<PropertyGuidValue, DomainPropertyGuidValue>().ForMember(dest => dest.PropertyType, opt => opt.Ignore())
                                                                    .ReverseMap()
                                                                    .ForMember(dest => dest.Size, opt => opt.Ignore());

      config.CreateMap<PropertyNameValue, DomainPropertyNameValue>().Include<PropertyByteValue, DomainPropertyByteValue>()
                                                                    .ForMember(dest => dest.PropertyType, opt => opt.Ignore())
                                                                    .ForMember(dest => dest.Value,        opt => opt.ResolveUsing(src => src.Context.Engine.Mapper.Map<DomainNameTableIndex>(((PropertyNameValue)src.Value).Value)))
                                                                    .ReverseMap()
                                                                    .ForMember(dest => dest.Size,  opt => opt.Ignore())
                                                                    .ForMember(dest => dest.Value, opt => opt.ResolveUsing(src => src.Context.Engine.Mapper.Map<NameTableIndex>(((DomainPropertyNameValue)src.Value).Value)));

      config.CreateMap<PropertyByteValue, DomainPropertyByteValue>().ForMember(dest => dest.PropertyType, opt => opt.Ignore())
                                                                    .ForMember(dest => dest.Value,        opt => opt.ResolveUsing(src => src.Context.Engine.Mapper.Map<DomainNameTableIndex>(((PropertyByteValue)src.Value).Value)))
                                                                    .ReverseMap()
                                                                    .ForMember(dest => dest.Size,  opt => opt.Ignore())
                                                                    .ForMember(dest => dest.Value, opt => opt.ResolveUsing(src => src.Context.Engine.Mapper.Map<NameTableIndex>(((DomainPropertyByteValue)src.Value).Value)));

      config.CreateMap<PropertyStrValue, DomainPropertyStrValue>().ForMember(dest => dest.PropertyType, opt => opt.Ignore())
                                                                  .ForMember(dest => dest.Value,        opt => opt.ResolveUsing(src => src.Context.Engine.Mapper.Map<DomainString>(((PropertyStrValue)src.Value).Value)))
                                                                  .ReverseMap()
                                                                  .ForMember(dest => dest.Size,  opt => opt.Ignore())
                                                                  .ForMember(dest => dest.Value, opt => opt.ResolveUsing(src => src.Context.Engine.Mapper.Map<UpkString>(((DomainPropertyStrValue)src.Value).Value)));

      config.CreateMap<PropertyStructValue, DomainPropertyStructValue>().ForMember(dest => dest.PropertyType, opt => opt.Ignore())
                                                                        .ReverseMap()
                                                                        .ForMember(dest => dest.Size, opt => opt.Ignore());

      config.CreateMap<PropertyArrayValue, DomainPropertyArrayValue>().ForMember(dest => dest.PropertyType, opt => opt.Ignore())
                                                                      .ReverseMap()
                                                                      .ForMember(dest => dest.Size, opt => opt.Ignore());

      #endregion Properties

    }

    #endregion IAutoMapperConfiguration Implementation

  }

}
