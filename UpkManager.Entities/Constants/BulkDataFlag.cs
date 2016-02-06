

namespace UpkManager.Entities.Constants {

  public static class BulkDataFlag {

    public const uint StoreInSeparateFile = 0x0001;

    public const uint CompressedZLib      = 0x0002;

    public const uint CompressedLzo       = 0x0010;

    public const uint Unused              = 0x0020;

    public const uint SeparateData        = 0x0040;

    public const uint CompressedLzx       = 0x0080;

    public const uint CompressedLzoEnc    = 0x0100;

  }

}
