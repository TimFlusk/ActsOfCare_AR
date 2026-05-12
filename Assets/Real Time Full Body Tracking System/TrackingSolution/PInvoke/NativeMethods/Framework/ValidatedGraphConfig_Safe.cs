

using System;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;

namespace TrackerPro
{
  internal static partial class SafeNativeMethods
  {
    [Pure, DllImport(TrackerProLibrary, ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.I1)]
    public static extern bool mp_ValidatedGraphConfig__Initialized(IntPtr config);

    [Pure, DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern int mp_ValidatedGraphConfig__OutputStreamIndex__PKc(IntPtr config, string name);

    [Pure, DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern int mp_ValidatedGraphConfig__OutputSidePacketIndex__PKc(IntPtr config, string name);

    [Pure, DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern int mp_ValidatedGraphConfig__OutputStreamToNode__PKc(IntPtr config, string name);

    [Pure, DllImport(TrackerProLibrary, ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.I1)]
    public static extern bool mp_ValidatedGraphConfig_IsReservedExecutorName(string name);

    [Pure, DllImport(TrackerProLibrary, ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.I1)]
    public static extern bool mp_ValidatedGraphConfig__IsExternalSidePacket__PKc(IntPtr config, string name);
  }
}
