

using System;
using System.Runtime.InteropServices;

namespace TrackerPro
{
  internal static partial class UnsafeNativeMethods
  {
    [DllImport(TrackerProLibrary)]
    public static extern void glFlush();

    [DllImport(TrackerProLibrary)]
    public static extern void glReadPixels(int x, int y, int width, int height, uint glFormat, uint glType, IntPtr pixels);
  }
}
