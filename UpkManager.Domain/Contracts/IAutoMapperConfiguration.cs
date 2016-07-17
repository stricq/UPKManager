using AutoMapper;


namespace UpkManager.Domain.Contracts {

  public interface IAutoMapperConfiguration {

    void RegisterMappings(IMapperConfigurationExpression config);

  }

}
