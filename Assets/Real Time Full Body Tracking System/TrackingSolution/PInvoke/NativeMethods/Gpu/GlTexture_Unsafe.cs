

using System;
using System.Runtime.InteropServices;

namespace TrackerPro
{
  internal static partial class UnsafeNativeMethods
  {
    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern MpReturnCode mp_GlTexture__(out IntPtr glTexture);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern void mp_GlTexture__delete(IntPtr glTexture);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern MpReturnCode mp_GlTexture__Release(IntPtr glTexture);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern MpReturnCode mp_GlTexture__GetGpuBufferFrame(IntPtr glTexture, out IntPtr gpuBuffer);
  }
}
