

using System;
using System.Runtime.InteropServices;

namespace TrackerPro
{
  internal static partial class UnsafeNativeMethods
  {
    #region GlContext
    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern void mp_SharedGlContext__delete(IntPtr sharedGlContext);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern void mp_SharedGlContext__reset(IntPtr sharedGlContext);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern MpReturnCode mp_GlContext_GetCurrent(out IntPtr sharedGlContext);
    #endregion

    #region GlSyncToken
    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern void mp_GlSyncToken__delete(IntPtr glSyncToken);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern void mp_GlSyncToken__reset(IntPtr glSyncToken);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern MpReturnCode mp_GlSyncPoint__Wait(IntPtr glSyncPoint);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern MpReturnCode mp_GlSyncPoint__WaitOnGpu(IntPtr glSyncPoint);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern MpReturnCode mp_GlSyncPoint__IsReady(IntPtr glSyncPoint, out bool value);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern MpReturnCode mp_GlSyncPoint__GetContext(IntPtr glSyncPoint, out IntPtr sharedGlContext);
    #endregion
  }
}
