

using System;
using System.Runtime.InteropServices;

namespace TrackerPro
{
  internal static partial class UnsafeNativeMethods
  {
    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern MpReturnCode absl_Status__i_PKc(int code, string message, out IntPtr status);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern void absl_Status__delete(IntPtr status);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern MpReturnCode absl_Status__ToString(IntPtr status, out IntPtr str);
  }
}
