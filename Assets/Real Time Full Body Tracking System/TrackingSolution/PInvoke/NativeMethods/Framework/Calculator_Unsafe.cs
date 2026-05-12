

using System.Runtime.InteropServices;

namespace TrackerPro
{
  internal static partial class UnsafeNativeMethods
  {
    [DllImport(TrackerProLibrary, ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.I1)]
    public static extern bool mp_api__ConvertFromCalculatorGraphConfigTextFormat(string configText, out SerializedProto serializedProto);
  }
}
