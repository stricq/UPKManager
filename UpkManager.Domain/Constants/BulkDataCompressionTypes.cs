using System;


namespace UpkManager.Domain.Constants {

  [Flags]
  public enum BulkDataCompressionTypes {

    StoreInSeparatefile = 0x00000001,

    CompressedZLib      = 0x00000002,

    CompressedLzo       = 0x00000010,

    Unused              = 0x00000020,

    SeperateData        = 0x00000040,

    CompressedLzx       = 0x00000080,

    CompressedLzoEnc    = 0x00000100

  }

}
