using AutoMapper;


namespace UpkManager.Domain.Contracts {

  public interface IAutoMapperConfiguration {

    void RegisterMappings(IMapperConfiguration config);

  }

}
