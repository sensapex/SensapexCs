using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SensapexCs
{
    /// <summary>
    /// Represents a class for controlling Umv devices, which are used for microfluidic pressure control.
    /// </summary>
    /// <seealso cref="Umx"/>
    public class Umv : Umx
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Ump"/> class.
        /// </summary>
        public Umv() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Ump"/> class with the specified device ID.
        /// </summary>
        /// <param name="devId">The device ID.</param>
        public Umv(int devId) : base(devId)
        {
        }

        [DllImport(Constants.UMSDK_FILEPATH, CallingConvention = CallingConvention.Cdecl)]
        protected static extern int umc_set_pressure_setting(IntPtr hndl, int dev, int channel, float value);

        [DllImport(Constants.UMSDK_FILEPATH, CallingConvention = CallingConvention.Cdecl)]
        protected static extern int umc_get_pressure_setting(IntPtr hndl, int dev, int channel, out float value);

        [DllImport(Constants.UMSDK_FILEPATH, CallingConvention = CallingConvention.Cdecl)]
        protected static extern int umc_measure_pressure(IntPtr hndl, int dev, int channel, out float value);

        [DllImport(Constants.UMSDK_FILEPATH, CallingConvention = CallingConvention.Cdecl)]
        protected static extern int umc_set_valve(IntPtr hndl, int dev, int channel, int value);

        [DllImport(Constants.UMSDK_FILEPATH, CallingConvention = CallingConvention.Cdecl)]
        protected static extern int umc_get_valve(IntPtr hndl, int dev, int channel);

        /// <summary>
        /// Sets the pressure value for a given channel.
        /// </summary>
        /// <param name="channel">The channel number.</param>
        /// <param name="value">The pressure value to set.</param>
        /// <returns>Returns true if the pressure value was set successfully, otherwise false.</returns>
        public bool SetPressure(int channel, float value)
        {
            return SetPressure(DevId, channel, value);
        }

        /// <summary>
        /// Sets the pressure value for a given channel on the specified Umv device.
        /// </summary>
        /// <param name="dev">The device ID.</param>
        /// <param name="channel">The channel number.</param>
        /// <param name="value">The pressure value to set.</param>
        /// <returns>Returns true if the pressure value was set successfully, otherwise false.</returns>
        public bool SetPressure(int dev, int channel, float value)
        {
            int result = Constants.LIBUM_ERROR_NOT_OPEN;
            if (validateState())
            {
                result = umc_set_pressure_setting(UmxHandle, dev, channel, value);
            }
            return result >= 0;
        }

        /// <summary>
        /// Gets the pressure value for a given channel.
        /// </summary>
        /// <param name="channel">The channel number.</param>
        /// <param name="value">The pressure value retrieved.</param>
        /// <returns>Returns true if the pressure value was retrieved successfully, otherwise false.</returns>
        public bool GetPressure(int channel, out float value)
        {
            return GetPressure(DevId, channel, out value);
        }

        /// <summary>
        /// Gets the pressure value for a given channel on the specified Umv device.
        /// </summary>
        /// <param name="dev">The device ID.</param>
        /// <param name="channel">The channel number.</param>
        /// <param name="value">The pressure value retrieved.</param>
        /// <returns>Returns true if the pressure value was retrieved successfully, otherwise false.</returns>
        public bool GetPressure(int dev, int channel, out float value)
        {
            float tmpValue = float.PositiveInfinity;
            int result = Constants.LIBUM_ERROR_NOT_OPEN;
            if (validateState())
            {
                result = umc_get_pressure_setting(UmxHandle, dev, channel, out tmpValue);
            }
            value = tmpValue;
            return result >= 0;
        }

        /// <summary>
        /// Measures the pressure value for a given channel.
        /// </summary>
        /// <param name="channel">The channel number.</param>
        /// <param name="value">The pressure value measured.</param>
        /// <returns>Returns true if the pressure value was measured successfully, otherwise false.</returns>
        public bool MeasurePressure(int channel, out float value)
        {
            return MeasurePressure(DevId, channel, out value);
        }

        /// <summary>
        /// Measures the pressure value for a given channel on the specified Umv device.
        /// </summary>
        /// <param name="dev">The device ID.</param>
        /// <param name="channel">The channel number.</param>
        /// <param name="value">The pressure value measured.</param>
        /// <returns>Returns true if the pressure value was measured successfully, otherwise false.</returns>
        public bool MeasurePressure(int dev, int channel, out float value)
        {
            float tmpValue = float.PositiveInfinity;
            int result = Constants.LIBUM_ERROR_NOT_OPEN;
            if (validateState())
            {
                result = umc_measure_pressure(UmxHandle, dev, channel, out tmpValue);
            }
            value = tmpValue;
            return result >= 0;
        }

        /// <summary>
        /// Sets the valve of the specified channel to the specified value.
        /// </summary>
        /// <param name="channel">The channel number of the valve to set.</param>
        /// <param name="value">The value to set the valve to.</param>
        /// <returns>True if the valve was successfully set, false otherwise.</returns>
        public bool SetValve(int channel, int value)
        {
            return SetValve(DevId, channel, value);
        }

        /// <summary>
        /// Sets the valve of the specified channel of the specified device to the specified value.
        /// </summary>
        /// <param name="dev">The device ID of the device to set the valve on.</param>
        /// <param name="channel">The channel number of the valve to set.</param>
        /// <param name="value">The value to set the valve to.</param>
        /// <returns>True if the valve was successfully set, false otherwise.</returns>
        public bool SetValve(int dev, int channel, int value)
        {
            int result = Constants.LIBUM_ERROR_NOT_OPEN;
            if (validateState())
            {
                result = umc_set_valve(UmxHandle, dev, channel, value);
            }
            return result >= 0;
        }

        /// <summary>
        /// Gets the value of the valve of the specified channel.
        /// </summary>
        /// <param name="channel">The channel number of the valve to get the value of.</param>
        /// <param name="value">The value of the valve, or int.MaxValue if the valve could not be read.</param>
        /// <returns>True if the valve value was successfully read, false otherwise.</returns>
        public bool GetValve(int channel, out int value)
        {
            return GetValve(DevId, channel, out value);
        }

        /// <summary>
        /// Gets the value of the valve of the specified channel on the specified Umv device.
        /// </summary>
        /// <param name="dev">The ID of the Umv device.</param>
        /// <param name="channel">The channel number of the valve to get the value of.</param>
        /// <param name="value">The value of the valve, or int.MaxValue if the valve could not be read.</param>
        /// <returns>True if the valve value was successfully read, false otherwise.</returns>
        public bool GetValve(int dev, int channel, out int value)
        {
            int tmpValue = int.MaxValue;
            int result = Constants.LIBUM_ERROR_NOT_OPEN;
            if (validateState())
            {
                result = umc_get_valve(UmxHandle, dev, channel);
                if (result >= 0)
                {
                    tmpValue = result;
                }
            }
            value = tmpValue;
            return result >= 0;
        }
    }
}
