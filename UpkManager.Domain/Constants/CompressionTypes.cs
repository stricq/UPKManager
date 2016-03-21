using System;
using System.Diagnostics.CodeAnalysis;


namespace UpkManager.Domain.Constants {

  [Flags]
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  public enum CompressionTypes : uint {

    ZLIB    = 0x00000001,
    LZO     = 0x00000002,
    LZX     = 0x00000004,
    LZO_ENC = 0x00000008

  }

}
