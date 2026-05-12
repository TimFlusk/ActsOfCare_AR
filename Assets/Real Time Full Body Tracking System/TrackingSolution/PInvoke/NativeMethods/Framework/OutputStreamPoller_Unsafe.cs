

using System;
using System.Runtime.InteropServices;

namespace TrackerPro
{
  internal static partial class UnsafeNativeMethods
  {
    #region OutputStreamPoller
    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern void mp_OutputStreamPoller__delete(IntPtr poller);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern MpReturnCode mp_OutputStreamPoller__Reset(IntPtr poller);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern MpReturnCode mp_OutputStreamPoller__Next_Ppacket(IntPtr poller, IntPtr packet, out bool result);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern MpReturnCode mp_OutputStreamPoller__SetMaxQueueSize(IntPtr poller, int queueSize);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern MpReturnCode mp_OutputStreamPoller__QueueSize(IntPtr poller, out int queueSize);
    #endregion
  }
}
