using System.Runtime.InteropServices;

namespace SensapexCs
{
    static class Constants
    {
        public const string UMSDK_FILEPATH = "libum";
        public const ushort LIBUM_MAX_DEVS = 0xFFFF;              /**< Max count of concurrent devices supported by this SDK version*/
        public const ushort LIBUM_MAX_LOG_LINE_LENGTH = 256;      /**< maximum log message length */
        public const int LIBUM_DEF_REFRESH_TIME = 20;
        public const int LIBUM_ERROR_NOT_OPEN = -2;
        public const string CS_SDK_VERSION = "0.9.4";
    }
    /// <summary>
    /// Represents information about the UmSdk library.
    /// </summary>
    public struct UmSdkInfo
    {
        /// <summary>
        /// Gets the version of the UmSdk library (libum.xxx).
        /// </summary>
        public string UmsdkVersion { get; set; }

        /// <summary>
        /// Gets the version of the Sensapex CS library.
        /// </summary>
        public string SensapexCsVersion { get; set; }
    }

    /// <summary>
    /// Provides info access to the UmSdk library basics.
    /// </summary>
    public static class UmSdk
    {
        [DllImport(Constants.UMSDK_FILEPATH)]
        private static extern IntPtr um_get_version();

        /// <summary>
        /// Gets information about the UmSdk library.
        /// </summary>
        /// <returns>An <see cref="UmSdkInfo"/> object containing information about the library.</returns>

        public static UmSdkInfo LibUmInfo()
        {
            string? resultStr = null;
            IntPtr resultPtr = um_get_version();
            if (resultPtr != IntPtr.Zero)
            {
                resultStr = Marshal.PtrToStringAnsi(resultPtr);
            }

            // Compose a response object
            UmSdkInfo RetVal = new()
            {
                UmsdkVersion = string.IsNullOrEmpty(resultStr) ? string.Empty : resultStr,
                SensapexCsVersion = Constants.CS_SDK_VERSION,
            };

            return RetVal;
        }
    }
}
