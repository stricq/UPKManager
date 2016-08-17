using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Helpers;
using UpkManager.Domain.Models.UpkFile.Tables;


namespace UpkManager.Domain.Models.UpkFile.Objects.Sounds {

  public class DomainObjectSoundNodeWave : DomainObjectCompressionBase {

    #region Constructor

    public DomainObjectSoundNodeWave() {
      Sounds = new List<byte[]>();
    }

    #endregion Constructor

    #region Properties

    public List<byte[]> Sounds { get; }

    public override bool IsExportable => true;

    public override ViewableTypes Viewable => ViewableTypes.Sound;

    public override ObjectTypes ObjectType => ObjectTypes.SoundNodeWave;

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
          byte[] ogg = (await bulkChunk.DecompressChunk(0))?.GetBytes();

          if (ogg == null || ogg.Length == 0) {
            done = true;

            return;
          }

          Sounds.Add(ogg);
        });
      } while(!done);
    }

    public override async Task SaveObject(string filename, object configuration) {
      if (!Sounds.Any()) return;

      await Task.Run(() => File.WriteAllBytes(filename, Sounds[0]));

      if (Sounds.Count == 1) return;

      string name = Path.GetFileNameWithoutExtension(filename);
      string ext  = Path.GetExtension(filename);

      for(int i = 1; i < Sounds.Count; ++i) {
        string soundFilename = Path.Combine(Path.GetDirectoryName(filename), $"{name}_{i}{ext}");

        int i1 = i;

        await Task.Run(() => File.WriteAllBytes(soundFilename, Sounds[i1]));
      }
    }

    public override Stream GetObjectStream() {
      return Sounds.Any() ? new MemoryStream(Sounds[0]) : null;
    }

    #endregion Domain Methods

  }

}
