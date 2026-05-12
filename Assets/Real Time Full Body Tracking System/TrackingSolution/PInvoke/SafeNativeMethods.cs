

using System.Security;

namespace TrackerPro
{
  [SuppressUnmanagedCodeSecurity]
  internal static partial class SafeNativeMethods
  {
    internal const string TrackerProLibrary =
#if UNITY_EDITOR
      "mediapipe_c";
#elif UNITY_IOS || UNITY_WEBGL
      "__Internal";
#elif UNITY_ANDROID
      "mediapipe_jni";
#else
      "mediapipe_c";
#endif
  }
}
