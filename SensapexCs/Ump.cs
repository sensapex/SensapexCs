// See https://aka.ms/new-console-template for more information

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static SensapexCs.Ump;


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

        [DllImport(Constants.UMSDK_FILEPATH, CallingConvention = CallingConvention.Cdecl)]
        protected static extern int um_take_step(IntPtr hndl, int dev,
            float stepX_um, float stepY_um, float stepZ_um, float stepD_um,
            int speedX_ums, int speedY_ums, int speedZ_ums, int speedD_ums,
            int mode, int max_accelaration);

        [DllImport(Constants.UMSDK_FILEPATH, CallingConvention = CallingConvention.Cdecl)]
        protected static extern int um_get_drive_status(IntPtr hndl, int dev);

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
            if (ValidateState())
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
            if (ValidateState())
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
            if (ValidateState())
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
            if (ValidateState())
            {
                int mode = simultaneous ? 1 : 0;
                // Request device to inform when it is done. We use this to sync the operation.
                CmdOptions(Smcpv1Constants.SMCP1_OPT_REQ_NOTIFY);
                // Move it!
                result = um_goto_position_ext(UmxHandle, dev, speedPos.xPositionUm, speedPos.yPositionUm,
                                              speedPos.zPositionUm, speedPos.wPositionUm, speedPos.xSpeedUms,
                                              speedPos.ySpeedUms, speedPos.zSpeedUms, speedPos.wSpeedUms,
                                              mode, max_acc);
                if (result >= 0)
                {
                    do
                    {
                        Recv(0);
                        result = GetDriveStatus(dev);
                    } while (result == Smcpv1Constants.LIBUM_POS_DRIVE_BUSY);
                }
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
            if (ValidateState())
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


        /// <summary>
        /// The possible step modes that can be used when taking steps.
        /// </summary>
        public enum StepMode
        {
            /// <summary>
            /// Automatic mode, where the device selects the best mode based on the step parameters.
            /// </summary>
            Automatic = 0,

            /// <summary>
            /// Custom low mode, where the device uses a low custom mode.
            /// </summary>
            CustomModeLow = 1,

            /// <summary>
            /// Custom high mode, where the device uses a high custom mode.
            /// </summary>
            CustomModeHigh = 2,
        }

        /// <summary>
        /// The possible target axes that steps can be taken on.
        /// </summary>
        public enum TargetAxis
        {
            /// <summary>
            /// The X axis.
            /// </summary>
            AxisX = 0,

            /// <summary>
            /// The Y axis.
            /// </summary>
            AxisY = 1,

            /// <summary>
            /// The Z axis.
            /// </summary>
            AxisZ = 2,

            /// <summary>
            /// The D axis.
            /// </summary>
            AxisD = 3,
        }

        /// <summary>
        /// Represents a single step to be taken on a target axis.
        /// </summary>
        public struct TargetStep
        {
            /// <summary>
            /// The axis to take the step on.
            /// </summary>
            public TargetAxis AxisId;

            /// <summary>
            /// The step size in micrometers.
            /// </summary>
            public float Step_um;

            /// <summary>
            /// The speed of the step in micrometers per second.
            /// </summary>
            public float Speed_ums;
        }

        /// <summary>
        /// Takes a step on the device using the specified list of target steps.
        /// </summary>
        /// <param name="targets">The list of target steps to take.</param>
        /// <param name="mode">The step mode to use (optional, defaults to <see cref="StepMode.Automatic"/>).</param>
        /// <param name="maxAcceleration">The maximum acceleration to use (optional, defaults to 0).</param>
        /// <returns>True if the step was successful, false otherwise.</returns>
        public bool TakeStep(List<TargetStep> targets, StepMode mode = StepMode.Automatic, int maxAcceleration = 0)
        {
            return TakeStep(DevId, targets, mode, maxAcceleration);
        }

        /// <summary>
        /// Takes a step on the specified device using the specified list of target steps.
        /// </summary>
        /// <param name="dev">The ID of the device to take the step on.</param>
        /// <param name="targets">The list of target steps to take.</param>
        /// <param name="mode">The step mode to use (optional, defaults to <see cref="StepMode.Automatic"/>).</param>
        /// <param name="maxAcceleration">The maximum acceleration to use (optional, defaults to 0).</param>
        /// <returns>True if the step was successful, false otherwise.</returns>
        public bool TakeStep(int dev, List<TargetStep> targets, StepMode mode = StepMode.Automatic, int maxAcceleration = 0)
        {
            int result = Constants.LIBUM_ERROR_NOT_OPEN;
            if (ValidateState())
            {
                result = 0;

                float stepX_um = 0;
                float stepY_um = 0;
                float stepZ_um = 0;
                float stepD_um = 0;

                int speedX_ums = 0;
                int speedY_ums = 0;
                int speedZ_ums = 0;
                int speedD_ums = 0;

                foreach(TargetStep target in targets)
                {
                    switch(target.AxisId)
                    {
                        case TargetAxis.AxisX:
                            stepX_um = target.Step_um;
                            speedX_ums = (int)target.Speed_ums;
                            break;
                        case TargetAxis.AxisY:
                            stepY_um = target.Step_um;
                            speedY_ums = (int)target.Speed_ums;
                            break;
                        case TargetAxis.AxisZ:
                            stepZ_um = target.Step_um;
                            speedZ_ums = (int)target.Speed_ums;
                            break;
                        case TargetAxis.AxisD:
                            stepD_um = target.Step_um;
                            speedD_ums = (int)target.Speed_ums;
                            break;
                    }
                }
                if (result >= 0) {
                    result = um_take_step(UmxHandle, dev, stepX_um, stepY_um, stepZ_um, stepD_um,
                                                speedX_ums, speedY_ums, speedZ_ums, speedD_ums,
                                                (int)mode, maxAcceleration);
                }
            }
            return result >= 0;
        }

        /// <summary>
        /// Takes a step on the device with the given step size for each axis, and the specified speed.
        /// </summary>
        /// <param name="stepX_um">The step size in micrometers for the X axis. Set to 0 for no step taken.</param>
        /// <param name="stepY_um">The step size in micrometers for the Y axis. Set to 0 for no step taken.</param>
        /// <param name="stepZ_um">The step size in micrometers for the Z axis. Set to 0 for no step taken.</param>
        /// <param name="stepD_um">The step size in micrometers for the D axis. Set to 0 for no step taken.</param>
        /// <param name="speed_ums">The speed in micrometers per second for all axes.</param>
        /// <param name="mode">The mode for taking the step. Default is Automatic.</param>
        /// <param name="maxAcceleration">The maximum acceleration for taking the step. Default is 0.</param>
        /// <returns>Returns true if the step was successfully taken, otherwise false.</returns>
        public bool TakeStep(float stepX_um, float stepY_um, float stepZ_um, float stepD_um,
            int speed_ums, StepMode mode = StepMode.Automatic, int maxAcceleration = 0)
        {
            return TakeStep(DevId, stepX_um, stepY_um, stepZ_um, stepD_um, speed_ums, mode, maxAcceleration);
        }

        /// <summary>
        /// Takes a step on the specified device with the given step size for each axis, and the specified speed.
        /// </summary>
        /// <param name="dev">The device ID to take a step on.</param>
        /// <param name="stepX_um">The step size in micrometers for the X axis. Set to 0 for no step taken.</param>
        /// <param name="stepY_um">The step size in micrometers for the Y axis. Set to 0 for no step taken.</param>
        /// <param name="stepZ_um">The step size in micrometers for the Z axis. Set to 0 for no step taken.</param>
        /// <param name="stepD_um">The step size in micrometers for the D axis. Set to 0 for no step taken.</param>
        /// <param name="speed_ums">The speed in micrometers per second for all axes.</param>
        /// <param name="mode">The mode for taking the step. Default is Automatic.</param>
        /// <param name="maxAcceleration">The maximum acceleration for taking the step. Default is 0.</param>
        /// <returns>Returns true if the step was successfully taken, otherwise false.</returns>
        public bool TakeStep(int dev, float stepX_um, float stepY_um, float stepZ_um, float stepD_um, 
            int speed_ums, StepMode mode = StepMode.Automatic, int maxAcceleration = 0)
        {
            int result = Constants.LIBUM_ERROR_NOT_OPEN;
            if (ValidateState())
            {
                result = um_take_step(UmxHandle, dev, stepX_um, stepY_um, stepZ_um, stepD_um,
                            speed_ums, speed_ums, speed_ums, speed_ums,
                            (int)mode, maxAcceleration);
            }
            return result >= 0;
        }

        /// <summary>
        /// Obtain memory or position drive status.
        /// </summary>
        /// <returns>
        /// Status of the selected device:
        /// - Constants.LIBUM_POS_DRIVE_COMPLETED
        /// - Constants.LIBUM_POS_DRIVE_BUSY
        /// - Constants.LIBUM_POS_DRIVE_FAILED
        /// </returns>
        public int GetDriveStatus()
        {
            return GetDriveStatus(DevId);
        }

        /// <summary>
        /// Obtain memory or position drive status on the specifid device.
        /// </summary>
        /// <param name="dev">Device ID</param>
        /// <returns>
        /// Status of the selected device:
        /// - Constants.LIBUM_POS_DRIVE_COMPLETED
        /// - Constants.LIBUM_POS_DRIVE_BUSY
        /// - Constants.LIBUM_POS_DRIVE_FAILED
        /// </returns>
        public int GetDriveStatus(int dev)
        {
            int result = Constants.LIBUM_ERROR_NOT_OPEN;
            if (ValidateState())
            {
                result = um_get_drive_status(UmxHandle, dev);
            }
            return result;
        }
    }
}
