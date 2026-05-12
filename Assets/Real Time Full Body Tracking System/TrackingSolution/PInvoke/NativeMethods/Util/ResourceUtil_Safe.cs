

using System.Runtime.InteropServices;

namespace TrackerPro
{
  internal static partial class SafeNativeMethods
  {
    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern void mp__SetCustomGlobalResourceProvider__P(
        [MarshalAs(UnmanagedType.FunctionPtr)] ResourceUtil.NativeResourceProvider provider);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern void mp__SetCustomGlobalPathResolver__P(
        [MarshalAs(UnmanagedType.FunctionPtr)] ResourceUtil.PathResolver resolver);
  }
}
