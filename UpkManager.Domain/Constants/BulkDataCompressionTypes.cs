using System;
using System.Diagnostics.CodeAnalysis;


namespace UpkManager.Domain.Constants {

  [Flags]
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  public enum BulkDataCompressionTypes : uint {

    StoreInSeparatefile = 0x00000001,
    ZLIB                = 0x00000002,
    LZO                 = 0x00000010,
    Unused              = 0x00000020,
    SeperateData        = 0x00000040,
    LZX                 = 0x00000080,
    LZO_ENC             = 0x00000100

  }

}
