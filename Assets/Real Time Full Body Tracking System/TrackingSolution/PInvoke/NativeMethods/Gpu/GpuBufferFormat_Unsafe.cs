

using System.Runtime.InteropServices;

namespace TrackerPro
{
  internal static partial class UnsafeNativeMethods
  {
    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern MpReturnCode mp__GlTextureInfoForGpuBufferFormat__ui_i_ui(
        GpuBufferFormat format, int plane, GlVersion glVersion, out GlTextureInfo glTextureInfo);
  }
}
