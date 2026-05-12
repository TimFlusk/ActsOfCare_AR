

using System;
using System.Runtime.InteropServices;

namespace TrackerPro
{
  internal static partial class UnsafeNativeMethods
  {
    #region GlTextureBuffer
    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern MpReturnCode mp_GlTextureBuffer__WaitUntilComplete(IntPtr glTextureBuffer);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern MpReturnCode mp_GlTextureBuffer__WaitOnGpu(IntPtr glTextureBuffer);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern MpReturnCode mp_GlTextureBuffer__Reuse(IntPtr glTextureBuffer);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern MpReturnCode mp_GlTextureBuffer__Updated__Pgst(IntPtr glTextureBuffer, IntPtr prodToken);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern MpReturnCode mp_GlTextureBuffer__DidRead__Pgst(IntPtr glTextureBuffer, IntPtr consToken);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern MpReturnCode mp_GlTextureBuffer__WaitForConsumers(IntPtr glTextureBuffer);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern MpReturnCode mp_GlTextureBuffer__WaitForConsumersOnGpu(IntPtr glTextureBuffer);
    #endregion

    #region SharedGlTextureBuffer
    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern void mp_SharedGlTextureBuffer__delete(IntPtr glTextureBuffer);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern void mp_SharedGlTextureBuffer__reset(IntPtr glTextureBuffer);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern MpReturnCode mp_SharedGlTextureBuffer__ui_ui_i_i_ui_PF_PSgc(
        uint target, uint name, int width, int height, GpuBufferFormat format,
        [MarshalAs(UnmanagedType.FunctionPtr)] GlTextureBuffer.DeletionCallback deletionCallback,
        IntPtr producerContext, out IntPtr sharedGlTextureBuffer);
    #endregion
  }
}
