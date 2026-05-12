

using System;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;

namespace TrackerPro
{
  internal static partial class SafeNativeMethods
  {
#if UNITY_STANDALONE_LINUX || UNITY_ANDROID
    [Pure, DllImport(TrackerProLibrary)]
    public static extern IntPtr eglGetCurrentContext();
#endif
  }
}
