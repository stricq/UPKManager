using System.Threading.Tasks;

using UpkManager.Domain.Models;


namespace UpkManager.Domain.Contracts {

  public interface ISettingsRepository {

    Task<DomainSettings> LoadSettingsAsync();

    Task SaveSettings(DomainSettings Settings);

  }

}
