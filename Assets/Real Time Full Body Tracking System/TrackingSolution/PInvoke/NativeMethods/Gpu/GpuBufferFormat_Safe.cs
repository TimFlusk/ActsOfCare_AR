

using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;

namespace TrackerPro
{
  internal static partial class SafeNativeMethods
  {
    [Pure, DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern ImageFormat.Types.Format mp__ImageFormatForGpuBufferFormat__ui(GpuBufferFormat format);

    [Pure, DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern ImageFormat.Types.Format mp__GpuBufferFormatForImageFormat__ui(ImageFormat.Types.Format format);
  }
}
