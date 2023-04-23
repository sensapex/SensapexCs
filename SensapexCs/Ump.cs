// See https://aka.ms/new-console-template for more information

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace SensapexCs
{
    /// <summary>
    /// Represents a structure for holding speed and position information for a UMP micromanipulator.
    /// </summary>
    public struct UmpSpeedPos
    {
        /// <summary>
        /// The X position in micrometers (um).
        /// </summary>
        public float xPositionUm;

        /// <summary>
        /// The Y position in micrometers (um).
        /// </summary>
        public float yPositionUm;

        /// <summary>
        /// The Z position in micrometers (um).
        /// </summary>
        public float zPositionUm;

        /// <summary>
        /// The W position in micrometers (um).
        /// </summary>
        public float wPositionUm;

        /// <summary>
        /// The X speed in micrometers per second (ums).
        /// </summary>
        public float xSpeedUms;

        /// <summary>
        /// The Y speed in micrometers per second (ums).
        /// </summary>
        public float ySpeedUms;

        /// <summary>
        /// The Z speed in micrometers per second (ums).
        /// </summary>
        public float zSpeedUms;

        /// <summary>
        /// The W speed in micrometers per second (ums).
        /// </summary>
        public float wSpeedUms;

        /// <summary>
        /// Initializes a new instance of the <see cref="UmpSpeedPos"/> struct.
        /// </summary>
        public UmpSpeedPos()
        {
            xPositionUm = float.NaN;
            yPositionUm = float.NaN;
            zPositionUm = float.NaN;
            wPositionUm = float.NaN;
            xSpeedUms = float.NaN;
            ySpeedUms = float.NaN;
            zSpeedUms = float.NaN;
            wSpeedUms = float.NaN;
        }
    }

    /// <summary>
    /// Represents a class for controlling a UMP micromanipulator.
    /// </summary>
    public class Ump : Umx
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Ump"/> class.
        /// </summary>
        public Ump() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Ump"/> class with the specified device ID.
        /// </summary>
        /// <param name="devId">The device ID.</param>
        public Ump(int devId) : base(devId)
        {
        }

        [DllImport(Constants.UMSDK_FILEPATH, CallingConvention = CallingConvention.Cdecl)]
        protected static extern int um_goto_position_ext(IntPtr hndl, int dev, float x, float y,
            float z, float d, float speedX, float speedY,
            float speedZ, float speedD, int mode, int max_acc);

        [DllImport(Constants.UMSDK_FILEPATH, CallingConvention = CallingConvention.Cdecl)]
        protected static extern int um_get_positions(IntPtr hndl, int dev, int time_limit,
            out float x, out float y, out float z, out float d, out int elapsedptr);

        [DllImport(Constants.UMSDK_FILEPATH, CallingConvention = CallingConvention.Cdecl)]
        protected static extern int ump_calibrate_load(IntPtr hndl, int dev);

        [DllImport(Constants.UMSDK_FILEPATH, CallingConvention = CallingConvention.Cdecl)]
        protected static extern int um_init_zero(IntPtr hndl, int dev, int axisMask);

        /// <summary>
        /// Calibrates the load for the current device.
        /// </summary>
        /// <returns>True if the operation succeeds, otherwise false.</returns>
        public bool CalibrateLoad()
        {
            return CalibrateLoad(DevId);
        }

        /// <summary>
        /// Calibrates the load for the specified device.
        /// </summary>
        /// <param name="dev">The device ID.</param>
        /// <returns>True if the operation succeeds, otherwise false.</returns>
        public bool CalibrateLoad(int dev)
        {
            int result = Constants.LIBUM_ERROR_NOT_OPEN;
            if (validateState())
            {
                result = ump_calibrate_load(UmxHandle, dev);
            }
            return result >= 0;
        }

        /// <summary>
        /// Gets the firmware version for the current device.
        /// </summary>
        /// <param name="version">A list to store the firmware version information.</param>
        /// <returns>True if the operation succeeds, otherwise false.</returns>
        public bool GetFirmwareVersion(List<int> version)
        {
            return GetFirmwareVersion(DevId, version);
        }

        /// <summary>
        /// Gets the firmware version for the specified device.
        /// </summary>
        /// <param name="dev">The device ID.</param>
        /// <param name="version">A list to store the firmware version information.</param>
        /// <returns>True if the operation succeeds, otherwise false.</returns>
        public bool GetFirmwareVersion(int dev, List<int> version)
        {
            int versionSegments = version.Capacity > 0 ? version.Capacity : 4;
            int[] tmpVersion = new int[versionSegments];

            int result = Constants.LIBUM_ERROR_NOT_OPEN;
            if (validateState())
            {
                result = um_read_version(UmxHandle, dev, tmpVersion, tmpVersion.Length);
                if (result >= 0)
                {
                    for (int i = 0; i < tmpVersion.Length; i++)
                    {
                        version.Add(tmpVersion[i]);
                    }
                }
            }
            return result >= 0;
        }

        /// <summary>
        /// Calibrates the zero position for the current device and specified axis mask.
        /// </summary>
        /// <param name="axisMask">The axis mask.</param>
        /// <returns>True if the operation succeeds, otherwise false.</returns>
        public bool CalibrateZeroPosition(int axisMask = 0b1111)
        {
            return CalibrateZeroPosition(DevId, axisMask);
        }

        /// <summary>
        /// Calibrates the zero position for the specified device and axis mask.
        /// </summary>
        /// <param name="dev">The device ID.</param>
        /// <param name="axisMask">The axis mask.</param>
        /// <returns>True if the operation succeeds, otherwise false.</returns>
        public bool CalibrateZeroPosition(int dev, int axisMask = 0b1111)
        {
            int result = Constants.LIBUM_ERROR_NOT_OPEN;
            if (validateState())
            {
                result = um_init_zero(UmxHandle, dev, axisMask);
            }
            return result >= 0;
        }

        /// <summary>
        /// Moves the micromanipulator to the specified position and speed.
        /// </summary>
        /// <param name="speedPos">The speed and position information.</param>
        /// <param name="simultaneous">Whether the movement should be simultaneous.</param>
        /// <param name="max_acc">The maximum acceleration.</param>
        /// <returns><c>true</c> if the movement was successful; otherwise, <c>false</c>.</returns>
        public bool GotoPosition(UmpSpeedPos speedPos, bool simultaneous = true, int max_acc = 0)
        {
            return GotoPosition(DevId, speedPos, simultaneous, max_acc);
        }

        /// <summary>
        /// Moves the micromanipulator with the specified device ID to the specified position and speed.
        /// </summary>
        /// <param name="dev">The device ID.</param>
        /// <param name="speedPos">The speed and position information.</param>
        /// <param name="simultaneous">Whether the movement should be simultaneous.</param>
        /// <param name="max_acc">The maximum acceleration.</param>
        /// <returns><c>true</c> if the movement was successful; otherwise, <c>false</c>.</returns>
        public bool GotoPosition(int dev, UmpSpeedPos speedPos, bool simultaneous = true, int max_acc = 0)        /// <param name="dev">The 
        {
            int result = Constants.LIBUM_ERROR_NOT_OPEN;
            if (validateState())
            {
                int mode = simultaneous ? 1 : 0;
                result = um_goto_position_ext(UmxHandle, dev, speedPos.xPositionUm, speedPos.yPositionUm,
                                              speedPos.zPositionUm, speedPos.wPositionUm, speedPos.xSpeedUms,
                                              speedPos.ySpeedUms, speedPos.zSpeedUms, speedPos.wSpeedUms,
                                              mode, max_acc);
            }
            return result >= 0;
        }

        /// <summary>
        /// Gets the current position for the current device.
        /// </summary>
        /// <param name="outPositions">A list to store the position information.</param>
        /// <param name="timeLimit">The time limit to wait for a response from the device, in milliseconds. Default is 1000ms.</param>
        /// <returns>True if the operation succeeds, otherwise false.</returns>
        public bool GetPositions(List<float> outPositions, int timeLimit = Constants.LIBUM_DEF_REFRESH_TIME)
        {
            return GetPositions(DevId, outPositions, timeLimit);
        }

        /// <summary>
        /// Gets the current position for the specified device ID.
        /// </summary>
        /// <param name="dev">The device ID.</param>
        /// <param name="outPositions">A list to store the position information.</param>
        /// <param name="timeLimit">The time limit to wait for a response from the device, in milliseconds. Default is 1000ms.</param>
        /// <returns>True if the operation succeeds, otherwise false.</returns>
        public bool GetPositions(int dev, List<float> outPositions, int timeLimit = Constants.LIBUM_DEF_REFRESH_TIME)
        {
            int result = Constants.LIBUM_ERROR_NOT_OPEN;
            if (validateState())
            {
                result = um_get_positions(UmxHandle, dev, timeLimit,
                    out float tmpX, out float tmpY, out float tmpZ, out float tmpD, out _);

                if (result >= 0)
                {
                    outPositions.Add(tmpX);
                    outPositions.Add(tmpY);
                    outPositions.Add(tmpZ);
                    outPositions.Add(tmpD);
                }
            }
            return result >= 0;
        }

    }
}
