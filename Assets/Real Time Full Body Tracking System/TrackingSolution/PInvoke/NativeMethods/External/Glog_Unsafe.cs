

using System.Runtime.InteropServices;

namespace TrackerPro
{
  internal static partial class UnsafeNativeMethods
  {
    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern MpReturnCode google_InitGoogleLogging__PKc(string name);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern MpReturnCode google_ShutdownGoogleLogging();

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern void glog_FLAGS_logtostderr([MarshalAs(UnmanagedType.I1)] bool value);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern void glog_FLAGS_stderrthreshold(int threshold);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern void glog_FLAGS_minloglevel(int level);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern void glog_FLAGS_log_dir(string dir);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern void glog_FLAGS_v(int v);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern void glog_LOG_INFO__PKc(string str);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern void glog_LOG_WARNING__PKc(string str);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern void glog_LOG_ERROR__PKc(string str);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern void glog_LOG_FATAL__PKc(string str);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern void google_FlushLogFiles(Glog.Severity severity);
  }
}
