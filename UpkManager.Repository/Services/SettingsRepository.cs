using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;

using Newtonsoft.Json;

using STR.Common.Extensions;

using UpkManager.Domain.Contracts;
using UpkManager.Domain.Models;


namespace UpkManager.Repository.Services {

  [Export(typeof(ISettingsRepository))]
  public sealed class SettingsRepository : ISettingsRepository {

    #region ISettingsRepository Implementation

    public async Task<DomainSettings> LoadSettingsAsync() {
      DomainSettings settings;

      if (await Task.Run(() => File.Exists(Filename))) {
        settings = await Task.Run(() => JsonConvert.DeserializeObject<DomainSettings>(File.ReadAllText(Filename)));

        if (settings.WindowW.EqualInPercentRange(0)) {
          settings.WindowW = 1280;
          settings.WindowH = 720;

          settings.WindowX = 100;
          settings.WindowY = 100;
        }
      }
      else settings = new DomainSettings {
        WindowW = 1280,
        WindowH = 720,

        WindowX = 100,
        WindowY = 100
      };

      return settings;
    }

    public async Task SaveSettings(DomainSettings Settings) {
      string json = await Task.Run(() => JsonConvert.SerializeObject(Settings, Formatting.Indented));

      if (!await Task.Run(() => File.Exists(Filename))) await Task.Run(() => Directory.CreateDirectory(Path.GetDirectoryName(Filename)));

      await Task.Run(() => File.WriteAllText(Filename, json));
    }

    #endregion ISettingsRepository Implementation

    #region Private Properties

    private static string Filename => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"STR Programming Services\UPK Manager\Settings.json");

    #endregion Private Properties

  }

}
