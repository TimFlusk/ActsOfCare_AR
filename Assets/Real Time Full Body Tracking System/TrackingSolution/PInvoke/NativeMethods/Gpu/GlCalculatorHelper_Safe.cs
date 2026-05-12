

using System;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;

namespace TrackerPro
{
  internal static partial class SafeNativeMethods
  {
    [Pure, DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern uint mp_GlCalculatorHelper__framebuffer(IntPtr glCalculatorHelper);

    [Pure, DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern IntPtr mp_GlCalculatorHelper__GetGlContext(IntPtr glCalculatorHelper);

    [Pure, DllImport(TrackerProLibrary, ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.I1)]
    public static extern bool mp_GlCalculatorHelper__Initialized(IntPtr glCalculatorHelper);
  }
}
