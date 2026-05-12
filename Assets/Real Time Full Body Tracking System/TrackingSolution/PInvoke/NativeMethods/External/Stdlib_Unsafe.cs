

using System;
using System.Runtime.InteropServices;

namespace TrackerPro
{
  internal static partial class UnsafeNativeMethods
  {
    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern void delete_array__PKc(IntPtr str);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern void delete_array__Pf(IntPtr str);

    #region String
    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern void std_string__delete(IntPtr str);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern MpReturnCode std_string__PKc_i(byte[] bytes, int size, out IntPtr str);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern void std_string__swap__Rstr(IntPtr src, IntPtr dst);
    #endregion
  }
}
