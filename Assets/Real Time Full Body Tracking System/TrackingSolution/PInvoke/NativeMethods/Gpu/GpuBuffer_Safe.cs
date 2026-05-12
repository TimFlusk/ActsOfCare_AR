

using System;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;

namespace TrackerPro
{
  internal static partial class SafeNativeMethods
  {
    [Pure, DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern int mp_GpuBuffer__width(IntPtr gpuBuffer);

    [Pure, DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern int mp_GpuBuffer__height(IntPtr gpuBuffer);

    [Pure, DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern GpuBufferFormat mp_GpuBuffer__format(IntPtr gpuBuffer);
  }
}
