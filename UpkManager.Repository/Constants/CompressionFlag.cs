using System.Diagnostics.CodeAnalysis;


namespace UpkManager.Repository.Constants {

  [SuppressMessage("ReSharper", "InconsistentNaming")]
  public static class CompressionFlag {

    public const uint ZLIB    = 0x00000001;

    public const uint LZO     = 0x00000002;

    public const uint LZX     = 0x00000004;

    public const uint LZO_ENC = 0x00000008;

  }

}
