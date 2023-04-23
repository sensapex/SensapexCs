using System.Runtime.InteropServices;

namespace SensapexCs
{
    public struct UmSdkInfo
    {
        public string Version;
    };
    
    public static class UmSdk
    {
        [DllImport(Constants.UMSDK_FILEPATH)]
        private static extern IntPtr um_get_version();

        public static UmSdkInfo LibUmInfo()
        {
            string? resultStr = null;
            IntPtr resultPtr = um_get_version();
            if (resultPtr != IntPtr.Zero)
            {
                resultStr = Marshal.PtrToStringAnsi(resultPtr);
            }

            UmSdkInfo RetVal = new()
            {
                Version = string.IsNullOrEmpty(resultStr) ? string.Empty : resultStr
            };

            return RetVal;
        }
    }
}
