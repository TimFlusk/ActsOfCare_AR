

namespace TrackerPro.Unity.Experimental
{
  public readonly struct ImageTransformationOptions
  {
    public readonly bool flipHorizontally;
    public readonly bool flipVertically;
    public readonly RotationAngle rotationAngle;

    private ImageTransformationOptions(bool flipHorizontally, bool flipVertically, RotationAngle rotationAngle)
    {
      this.flipHorizontally = flipHorizontally;
      this.flipVertically = flipVertically;
      this.rotationAngle = rotationAngle;
    }

    public static ImageTransformationOptions Build(bool shouldFlipHorizontally, bool isVerticallyFlipped, RotationAngle rotation)
    {
      var isInverted = CoordinateSystem.ImageCoordinate.IsInverted(rotation);
      var flipHorizontally = !isInverted && shouldFlipHorizontally;
      var flipVertically = !shouldFlipHorizontally ? !isVerticallyFlipped : isInverted ? isVerticallyFlipped : !isVerticallyFlipped;

      return new ImageTransformationOptions(flipHorizontally, flipVertically, rotation);
    }
  }
}
