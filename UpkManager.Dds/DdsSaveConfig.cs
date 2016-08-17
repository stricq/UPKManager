using System;

using UpkManager.Dds.Constants;


namespace UpkManager.Dds {

  public sealed class DdsSaveConfig {

    #region Constructor

    public DdsSaveConfig(FileFormat fileFormat, int compressorType, int errorMetric, bool weightColorByAlpha, bool generateMipMaps) {
      FileFormat = fileFormat;

      CompressorType = compressorType;
      ErrorMetric    = errorMetric;

      WeightColorByAlpha = weightColorByAlpha;
      GenerateMipMaps    = generateMipMaps;
    }

    #endregion Constructor

    #region Properties

    public bool GenerateMipMaps { get; }

    public bool WeightColorByAlpha { get; }

    public int CompressorType { get; }

    public int ErrorMetric { get; }

    public FileFormat FileFormat { get; set; }

    #endregion Properties

    #region Public Methods

    internal int GetSquishFlags() {
      int squishFlags = 0;
      //
      // Translate file format
      //
      switch(FileFormat) {
        case FileFormat.DXT1:
          squishFlags |= (int)SquishFlags.Dxt1;
          break;
        case FileFormat.DXT3:
          squishFlags |= (int)SquishFlags.Dxt3;
          break;
        case FileFormat.DXT5:
          squishFlags |= (int)SquishFlags.Dxt5;
          break;
      }
      //
      // If this isn't a DXT file, then no flags
      //
      if (squishFlags == 0) return squishFlags;
      //
      // Translate compressor type
      //
      switch(CompressorType) {
        case 0:
          squishFlags |= (int)SquishFlags.ColourClusterFit;
          break;
        case 1:
          squishFlags |= (int)SquishFlags.ColourRangeFit;
          break;
        default:
          squishFlags |= (int)SquishFlags.ColourIterativeClusterFit;
          break;
      }
      //
      // Translate error metric
      //
      if (ErrorMetric == 0) squishFlags |= (int)SquishFlags.ColourMetricPerceptual;
      else squishFlags |= (int)SquishFlags.ColourMetricUniform;
      //
      // Now the colour weighting state (only valid for cluster fit)
      //
      if ((CompressorType == 0) && WeightColorByAlpha) squishFlags |= (int)SquishFlags.WeightColourByAlpha;

      return squishFlags;
    }

    #endregion Public Methods

  }

}
