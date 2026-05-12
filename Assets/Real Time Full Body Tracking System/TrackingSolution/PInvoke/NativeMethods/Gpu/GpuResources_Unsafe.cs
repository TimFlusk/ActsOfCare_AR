

using System;
using System.Runtime.InteropServices;

namespace TrackerPro
{
  internal static partial class UnsafeNativeMethods
  {
    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern void mp_SharedGpuResources__delete(IntPtr gpuResources);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern void mp_SharedGpuResources__reset(IntPtr gpuResources);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern MpReturnCode mp_GpuResources_Create(out IntPtr status, out IntPtr gpuResources);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern MpReturnCode mp_GpuResources_Create__Pv(IntPtr externalContext, out IntPtr status, out IntPtr gpuResources);
  }
}
