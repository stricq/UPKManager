using System.IO;
using System.Threading.Tasks;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Helpers;
using UpkManager.Domain.Models.UpkFile.Tables;


namespace UpkManager.Domain.Models.UpkFile.Objects.Textures {

  public sealed class DomainObjectTextureMovie : DomainObjectCompressionBase {

    #region Properties

    public byte[] Movie { get; private set; }

    public override bool IsExportable => true;

    public override ObjectTypes ObjectType => ObjectTypes.TextureMovie;

    public override string FileExtension => ".bik";

    public override string FileTypeDesc => "Bink Video";

    #endregion Properties

    #region Domain Methods

    public override async Task ReadDomainObject(ByteArrayReader reader, DomainHeader header, DomainExportTableEntry export, bool skipProperties, bool skipParse) {
      await base.ReadDomainObject(reader, header, export, skipProperties, skipParse);

      if (skipParse) return;

      await ProcessCompressedBulkData(reader, async bulkChunk => {
        byte[] bik = (await bulkChunk.DecompressChunk(0))?.GetBytes();

        if (bik == null || bik.Length == 0) return;

        Movie = bik;
      });
    }

    public override async Task SaveObject(string filename, object configuration) {
      if (Movie == null || Movie.Length == 0) return;

      await Task.Run(() => File.WriteAllBytes(filename, Movie));
    }

    #endregion Domain Methods

  }

}
