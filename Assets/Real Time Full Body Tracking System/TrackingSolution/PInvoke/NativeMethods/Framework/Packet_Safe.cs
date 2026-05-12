

using System;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;

namespace TrackerPro
{
  internal static partial class SafeNativeMethods
  {
    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.I1)]
    public static extern bool mp_Packet__IsEmpty(IntPtr packet);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern long mp_Packet__TimestampMicroseconds(IntPtr packet);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern void mp_PacketMap__clear(IntPtr packetMap);

    [Pure, DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern int mp_PacketMap__size(IntPtr packetMap);
  }
}
