

using System;
using System.Runtime.InteropServices;

namespace TrackerPro
{
  internal static partial class UnsafeNativeMethods
  {
    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern MpReturnCode mp_tasks_core_TaskRunner_Create__PKc_i_PF_Pgr(byte[] serializedConfig, int size,
        int callbackId, [MarshalAs(UnmanagedType.FunctionPtr)] Tasks.Core.TaskRunner.NativePacketsCallback packetsCallback,
        IntPtr gpuResources,
        out IntPtr status, out IntPtr taskRunner);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern MpReturnCode mp_tasks_core_TaskRunner_Create__PKc_i_PF(byte[] serializedConfig, int size,
        int callbackId, [MarshalAs(UnmanagedType.FunctionPtr)] Tasks.Core.TaskRunner.NativePacketsCallback packetsCallback,
        out IntPtr status, out IntPtr taskRunner);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern void mp_tasks_core_TaskRunner__delete(IntPtr taskRunner);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern MpReturnCode mp_tasks_core_TaskRunner__Process__Ppm(IntPtr taskRunner, IntPtr inputs, out IntPtr status, out IntPtr packetMap);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern MpReturnCode mp_tasks_core_TaskRunner__Send__Ppm(IntPtr taskRunner, IntPtr inputs, out IntPtr status);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern MpReturnCode mp_tasks_core_TaskRunner__Close(IntPtr taskRunner, out IntPtr status);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern MpReturnCode mp_tasks_core_TaskRunner__Restart(IntPtr taskRunner, out IntPtr status);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern MpReturnCode mp_tasks_core_TaskRunner__GetGraphConfig(IntPtr taskRunner, out SerializedProto serializedProto);
  }
}
