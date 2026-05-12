

using System;
using System.Runtime.InteropServices;

namespace TrackerPro
{
  internal static partial class UnsafeNativeMethods
  {
    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern MpReturnCode mp_Packet__GetClassifications(IntPtr packet, out NativeClassifications value);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern void mp_tasks_c_components_containers_CppCloseClassifications(NativeClassifications data);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern MpReturnCode mp_Packet__GetClassificationResult(IntPtr packet, out NativeClassificationResult value);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern MpReturnCode mp_Packet__GetClassificationsVector(IntPtr packet, out NativeClassificationResult value);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern void mp_tasks_c_components_containers_CppCloseClassificationResult(NativeClassificationResult data);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern MpReturnCode mp_Packet__GetClassificationResultVector(IntPtr packet, out NativeClassificationResultArray value);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern void mp_api_ClassificationResultArray__delete(NativeClassificationResultArray data);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern MpReturnCode mp_Packet__GetDetectionResult(IntPtr packet, out NativeDetectionResult value);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern void mp_tasks_c_components_containers_CppCloseDetectionResult(NativeDetectionResult data);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern MpReturnCode mp_Packet__GetLandmarks(IntPtr packet, out NativeLandmarks value);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern void mp_tasks_c_components_containers_CppCloseLandmarks(NativeLandmarks data);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern MpReturnCode mp_Packet__GetLandmarksVector(IntPtr packet, out NativeLandmarksArray value);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern void mp_api_LandmarksArray__delete(NativeLandmarksArray data);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern MpReturnCode mp_Packet__GetNormalizedLandmarks(IntPtr packet, out NativeNormalizedLandmarks value);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern void mp_tasks_c_components_containers_CppCloseNormalizedLandmarks(NativeNormalizedLandmarks data);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern MpReturnCode mp_Packet__GetNormalizedLandmarksVector(IntPtr packet, out NativeNormalizedLandmarksArray value);

    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    public static extern void mp_api_NormalizedLandmarksArray__delete(NativeNormalizedLandmarksArray data);
  }
}
