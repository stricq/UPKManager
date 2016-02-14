using System.Threading.Tasks;

using UpkManager.Domain.Models;


namespace UpkManager.Domain.Contracts {

  public interface ISettingsRepository {

    Task<DomainUpkManagerSettings> LoadSettingsAsync();

    Task SaveSettings(DomainUpkManagerSettings Settings);

  }

}
