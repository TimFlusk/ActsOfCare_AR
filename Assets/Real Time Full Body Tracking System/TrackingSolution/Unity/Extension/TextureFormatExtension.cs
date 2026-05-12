

using UnityEngine;

namespace TrackerPro
{
  public static class TextureFormatExtension
  {
    public static ImageFormat.Types.Format ToImageFormat(this TextureFormat textureFormat)
    {
#pragma warning disable IDE0010
      switch (textureFormat)
      {
        case TextureFormat.RGB24:
          {
            return ImageFormat.Types.Format.Srgb;
          }
        case TextureFormat.RGBA32:
          {
            return ImageFormat.Types.Format.Srgba;
          }
        case TextureFormat.Alpha8:
          {
            return ImageFormat.Types.Format.Gray8;
          }
        case TextureFormat.RGB48:
          {
            return ImageFormat.Types.Format.Srgb48;
          }
        case TextureFormat.RGBA64:
          {
            return ImageFormat.Types.Format.Srgba64;
          }
        case TextureFormat.RFloat:
          {
            return ImageFormat.Types.Format.Vec32F1;
          }
        case TextureFormat.RGFloat:
          {
            return ImageFormat.Types.Format.Vec32F2;
          }
        case TextureFormat.BGRA32:
          {
            return ImageFormat.Types.Format.Sbgra;
          }
        default:
          {
            return ImageFormat.Types.Format.Unknown;
          }
      }
    }
#pragma warning restore IDE0010
  }
}
