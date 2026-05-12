

using System;

#if UNITY_STANDALONE_LINUX || UNITY_ANDROID
namespace TrackerPro
{
  public class Egl
  {
    public static IntPtr GetCurrentContext()
    {
      return SafeNativeMethods.eglGetCurrentContext();
    }
  }
}
#endif
