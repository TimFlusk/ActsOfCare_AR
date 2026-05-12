

using System;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;

namespace TrackerPro
{
  internal static partial class SafeNativeMethods
  {
    [Pure, DllImport(TrackerProLibrary, ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.I1)]
    public static extern bool absl_Status__ok(IntPtr status);

    [Pure, DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern int absl_Status__raw_code(IntPtr status);
  }
}
