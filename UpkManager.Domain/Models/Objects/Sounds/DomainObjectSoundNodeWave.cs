using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Helpers;
using UpkManager.Domain.Models.Tables;


namespace UpkManager.Domain.Models.Objects.Sounds {

  public class DomainObjectSoundNodeWave : DomainObjectCompressionBase {

    #region Constructor

    public DomainObjectSoundNodeWave() {
      Sounds = new List<byte[]>();
    }

    #endregion Constructor

    #region Properties

    public List<byte[]> Sounds { get; set; }

    public override bool IsExportable => true;

    public override bool IsViewable => false;

    public override ObjectType ObjectType => ObjectType.SoundNodeWave;

    public override string FileExtension => ".ogg";

    public override string FileTypeDesc => "Ogg Vorbis";

    #endregion Properties

    #region Domain Methods

    public override async Task ReadDomainObject(ByteArrayReader reader, DomainHeader header, DomainExportTableEntry export, bool skipProperties, bool skipParse) {
      await base.ReadDomainObject(reader, header, export, skipProperties, skipParse);

      if (skipParse) return;

      bool done = false;

      do {
        await ProcessCompressedBulkData(reader, async bulkChunk => {
          byte[] ogg = (await bulkChunk.DecompressChunk(0))?.GetByteArray();

          if (ogg == null || ogg.Length == 0) {
            done = true;

            return;
          }

          Sounds.Add(ogg);
        });
      } while(!done);
    }

    public override async Task SaveObject(string filename) {
      if (Sounds.Count == 1) await Task.Run(() => File.WriteAllBytes(filename, Sounds[0]));

      string name = Path.GetFileNameWithoutExtension(filename);
      string ext  = Path.GetExtension(filename);

      for(int i = 0; i < Sounds.Count; ++i) {
        string soundFilename = Path.Combine(Path.GetFullPath(filename), $"{name}_{i}{ext}");

        int i1 = i;

        await Task.Run(() => File.WriteAllBytes(soundFilename, Sounds[i1]));
      }
    }

    #endregion Domain Methods

  }

}
