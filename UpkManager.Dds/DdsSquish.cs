using System;
using System.Runtime.InteropServices;

using UpkManager.Dds.Constants;


namespace UpkManager.Dds {

  internal static class DdsSquish {

    #region Private Fields

    private static readonly bool is64Bit = IntPtr.Size == 8;

    private const string SquishDll32     = @"lib32\Squish_x86.dll";
    private const string SquishDll32Sse2 = @"lib32\Squish_x86_SSE2.dll";
    private const string SquishDll64     = @"lib64\Squish_x64.dll";

    internal delegate void ProgressFn(int workDone, int workTotal);

    #endregion Private Fields

    #region Imports

    #region 32 Bit

    [DllImport(SquishDll32, EntryPoint = "SquishInitialize")]
    private static extern void SquishInitialize_32();

    [DllImport(SquishDll32, EntryPoint = "SquishCompressImage")]
    private static extern unsafe void SquishCompressImage_32(byte* rgba, int width, int height, byte* blocks, int flags, [MarshalAs(UnmanagedType.FunctionPtr)] ProgressFn progressFn);

    [DllImport(SquishDll32, EntryPoint = "SquishDecompressImage")]
    private static extern unsafe void SquishDecompressImage_32(byte* rgba, int width, int height, byte* blocks, int flags, [MarshalAs(UnmanagedType.FunctionPtr)] ProgressFn progressFn);

    #endregion 32 Bit

    #region 32 Bit SSE2

    [DllImport(SquishDll32Sse2, EntryPoint = "SquishInitialize")]
    private static extern void SquishInitialize_Sse2();

    [DllImport(SquishDll32Sse2, EntryPoint = "SquishCompressImage")]
    private static extern unsafe void SquishCompressImage_Sse2(byte* rgba, int width, int height, byte* blocks, int flags, [MarshalAs(UnmanagedType.FunctionPtr)] ProgressFn progressFn);

    [DllImport(SquishDll32Sse2, EntryPoint = "SquishDecompressImage")]
    private static extern unsafe void SquishDecompressImage_Sse2(byte* rgba, int width, int height, byte* blocks, int flags, [MarshalAs(UnmanagedType.FunctionPtr)] ProgressFn progressFn);

    #endregion 32 Bit SSE2

    #region 64 Bit

    [DllImport(SquishDll64, EntryPoint = "SquishInitialize")]
    private static extern void SquishInitialize_64();

    [DllImport(SquishDll64, EntryPoint = "SquishCompressImage")]
    private static extern unsafe void SquishCompressImage_64(byte* rgba, int width, int height, byte* blocks, int flags, [MarshalAs(UnmanagedType.FunctionPtr)] ProgressFn progressFn);

    [DllImport(SquishDll64, EntryPoint = "SquishDecompressImage")]
    private static extern unsafe void SquishDecompressImage_64(byte* rgba, int width, int height, byte* blocks, int flags, [MarshalAs(UnmanagedType.FunctionPtr)] ProgressFn progressFn);

    #endregion 64 Bit

    #endregion Imports

    #region Public Methods

    internal static void Initialize() {
      if (is64Bit) SquishInitialize_64();
      else SquishInitialize_32();
    }

    internal static unsafe byte[] CompressImage(byte[] rgba, int width, int height, int flags, ProgressFn progressFn) {
      int blockCount = (width + 3 ) / 4 * ((height + 3) / 4);

      int blockSize = (flags & (int)SquishFlags.Dxt1) != 0 ? 8 : 16;
      //
      // Allocate room for compressed blocks
      //
      byte[] blockData = new byte[blockCount * blockSize];

      fixed(byte *pRgba = rgba) {
        fixed(byte *pBlocks = blockData) {
          if (is64Bit) SquishCompressImage_64(pRgba, width, height, pBlocks, flags, progressFn);
          else SquishCompressImage_32(pRgba, width, height, pBlocks, flags, progressFn);
        }
      }

      GC.KeepAlive(progressFn);

      return blockData;
    }

    internal static unsafe byte[] DecompressImage(int width, int height, byte[] blocks, int flags, ProgressFn progressFn) {
      byte[] rgba = new byte[width * height * 4];

      fixed(byte *pRgba = rgba) {
        fixed(byte *pBlocks = blocks) {
          if (is64Bit) SquishDecompressImage_64(pRgba, width, height, pBlocks, flags, progressFn);
          else SquishDecompressImage_32(pRgba, width, height, pBlocks, flags, progressFn);
        }
      }

      GC.KeepAlive(progressFn);

      return rgba;
    }

    #endregion Public Methods

  }

}
