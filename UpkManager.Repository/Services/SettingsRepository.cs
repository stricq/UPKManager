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

    public async Task<DomainSettings> LoadSettingsAsync() {
      Settings settings;

      if (await Task.Run(() => File.Exists(Filename))) {
        settings = await Task.Run(() => JsonConvert.DeserializeObject<Settings>(File.ReadAllText(Filename)));
      }
      else settings = new Settings();

      return await Task.Run(() => mapper.Map<DomainSettings>(settings));
    }

    public async Task SaveSettings(DomainSettings Settings) {
      Settings settings = await Task.Run(() => mapper.Map<Settings>(Settings));

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
