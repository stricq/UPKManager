using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;

using AutoMapper;

using Newtonsoft.Json;

using UpkManager.Domain.Contracts;
using UpkManager.Domain.Models;

using UpkManager.Entities;


namespace UpkManager.Repository.Services {

  [Export(typeof(ISettingsRepository))]
  public class SettingsRepository : ISettingsRepository {

    #region Private Fields

    private readonly IMapper mapper;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public SettingsRepository(IMapper Mapper) {
      mapper = Mapper;
    }

    #endregion Constructor

    #region ISettingsRepository Implementation

    public async Task<DomainUpkManagerSettings> LoadSettingsAsync() {
      UpkManagerSettings settings;

      if (await Task.Run(() => File.Exists(Filename))) {
        settings = await Task.Run(() => JsonConvert.DeserializeObject<UpkManagerSettings>(File.ReadAllText(Filename)));
      }
      else settings = new UpkManagerSettings();

      return await Task.Run(() => mapper.Map<DomainUpkManagerSettings>(settings));
    }

    public async Task SaveSettings(DomainUpkManagerSettings Settings) {
      UpkManagerSettings settings = await Task.Run(() => mapper.Map<UpkManagerSettings>(Settings));

      string json = await Task.Run(() => JsonConvert.SerializeObject(settings, Formatting.Indented));

      if (!await Task.Run(() => File.Exists(Filename))) await Task.Run(() => Directory.CreateDirectory(Path.GetDirectoryName(Filename)));

      await Task.Run(() => File.WriteAllText(Filename, json));
    }

    #endregion ISettingsRepository Implementation

    #region Private Properties

    private static string Filename => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"STR Programming Services\UPK Manager\Settings.json");

    #endregion Private Properties

  }

}
