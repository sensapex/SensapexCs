using System.Runtime.InteropServices;

namespace SensapexCs
{
    public delegate void UmLogPrintFunc(int level, IntPtr arg, string func, string message);
}

namespace SensapexCs
{
    public struct um_positions
    {
        public int x;                         /**< X-actuator position */
        public int y;                         /**< Y-actuator position */
        public int z;                         /**< Z-actuator position */
        public int d;                         /**< D-actuator position */
        public float speed_x;                 /**< X-actuator movement speed between last two position updates */
        public float speed_y;                 /**< Y-actuator movement speed between last two position updates */
        public float speed_z;                 /**< Z-actuator movement speed between last two position updates */
        public float speed_d;                 /**< D-actuator movement speed between last two position updates */
        public ulong updated_us;              /**< Timestamp (in microseconds) when positions were updated */
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct IPADDR
    {
        public short sin_family;           // address family (AF_INET, AF_INET6, etc.)
        public ushort sin_port;            // port number
        public uint sin_addr;              // internet address
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] sin_zero;            // unused
    }

}

namespace SensapexCs
{
    [StructLayout(LayoutKind.Sequential)]

    public struct um_state
    {
        public uint last_received_time;
        public IntPtr socket;
        public int own_id;
        public ushort message_id;
        public int last_device_sent;
        public int last_device_received;
        public int retransmit_count;
        public int refresh_time_limit;
        public int last_error;
        public int last_os_errno;
        public int timeout;
        public int udp_port;
        public int local_port;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.LIBUM_MAX_DEVS)]
        public int[] last_status;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.LIBUM_MAX_DEVS)]
        public int[] drive_status;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.LIBUM_MAX_DEVS)]
        public ushort[] drive_status_id;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.LIBUM_MAX_DEVS)]
        public IPADDR[] addresses;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.LIBUM_MAX_DEVS)]
        public um_positions[] last_positions;
        public IPADDR laddr;
        public IPADDR raddr;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Constants.LIBUM_MAX_LOG_LINE_LENGTH)]
        public string errorstr_buffer;
        public int verbose;
        public UmLogPrintFunc log_func_ptr;
        public IntPtr log_print_arg;
        public int next_cmd_options;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.LIBUM_MAX_DEVS)]
        public ulong[] drive_status_ts;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.LIBUM_MAX_DEVS)]
        public ulong[] last_msg_ts;
    };
}

namespace SensapexCs
{
    /// <summary>
    /// Represents a class for working with UMX devices.
    /// </summary>
    /// <seealso cref="https://github.com/sensapex/umsdk/blob/master/include/libum.h"/>
    public class Umx
    {
        /// <summary>
        /// Initializes a new instance of the Umx class.
        /// </summary>
        public Umx()
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the Umx class with the specified device ID.
        /// </summary>
        /// <param name="devId">The device ID to use.</param>
        public Umx(int devId)
        {
            this.DevId = devId;
        }

        /// <summary>
        /// Finalizes an instance of the Umx class.
        /// </summary>
        ~Umx()
        {
            Close();
        }

        /// <summary>
        /// Gets or sets the device ID for the Umx instance.
        /// </summary>
        public int DevId { get; set; }

        /// <summary>
        /// Gets or sets the current state of the Umx instance.
        /// </summary>
        protected um_state UmState { set; get; }

        /// <summary>
        /// Gets or sets the handle for the Umx instance.
        /// </summary>
        protected IntPtr UmxHandle { set; get; }

        [DllImport(Constants.UMSDK_FILEPATH, CallingConvention = CallingConvention.Cdecl)]
        protected static extern void um_close(IntPtr hndl);

        [DllImport(Constants.UMSDK_FILEPATH, CallingConvention = CallingConvention.Cdecl)]
        protected static extern int um_get_device_list(IntPtr hndl, IntPtr outDevices, uint maxCount);

        [DllImport(Constants.UMSDK_FILEPATH, CallingConvention = CallingConvention.Cdecl)]
        protected static extern int um_get_param(IntPtr hndl, int dev, int param_id, out int value);

        [DllImport(Constants.UMSDK_FILEPATH, CallingConvention = CallingConvention.Cdecl)]
        protected static extern int um_is_busy(IntPtr hndl, int dev);

        [DllImport(Constants.UMSDK_FILEPATH, CallingConvention = CallingConvention.Cdecl)]
        protected static extern int um_last_error(IntPtr hndl);

        [DllImport(Constants.UMSDK_FILEPATH, CallingConvention = CallingConvention.Cdecl)]
        protected static extern int um_ping(IntPtr hndl, int dev);

        [DllImport(Constants.UMSDK_FILEPATH, CallingConvention = CallingConvention.Cdecl)]
        protected static extern int um_read_version(IntPtr hndl, int dev, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] int[] version, int size);

        [DllImport(Constants.UMSDK_FILEPATH, CallingConvention = CallingConvention.Cdecl)]
        protected static extern int um_set_param(IntPtr hndl, int dev, int param_id, int value);

        [DllImport(Constants.UMSDK_FILEPATH, CallingConvention = CallingConvention.Cdecl)]
        protected static extern int um_stop(IntPtr hndl, int dev);
        
        [DllImport(Constants.UMSDK_FILEPATH, CallingConvention = CallingConvention.Cdecl)]
        protected static extern IntPtr um_open(string udp_target_address, uint timeout, int group);

        [DllImport(Constants.UMSDK_FILEPATH, CallingConvention = CallingConvention.Cdecl)]
        protected static extern int um_cmd_options(IntPtr hndl,  int optionbits);
        /// <summary>
        /// Closes the Umx instance.
        /// </summary>
        public void Close()
        {
            if (validateState())
            {
                um_close(UmxHandle);
                UmxHandle = IntPtr.Zero;
                DevId = 0;
            }
        }

        /// <summary>
        /// Opens a new Umx instance with the specified UDP target address, timeout, and group.
        /// </summary>
        /// <param name="udp_target_address">The UDP target address to use.</param>
        /// <param name="timeout">The timeout to use.</param>
        /// <param name="group">The group to use.</param>
        /// <returns>true if the Umx instance was opened successfully; otherwise, false.</returns>
        /// <seealso cref="Close"/>
        public bool Open(string udp_target_address, uint timeout, int group)
        {
            UmxHandle = um_open(udp_target_address, timeout, group);
            bool success = UmxHandle != IntPtr.Zero;
            if (success)
            {
                UmState = Marshal.PtrToStructure<um_state>(UmxHandle);
            }
            return success;
        }

        /// <summary>
        /// Queries the Umx instance for a list of devices.
        /// </summary>
        /// <param name="maxCount">The maximum number of devices to return.</param>
        /// <returns>A list of device IDs found by the query.</returns>
        /// <seealso cref="DevId"/>
        public List<int> QueryDevices(uint maxCount)
        {
            List<int> deviceList = new List<int>();
            if (validateState())
            {
                IntPtr dstPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(uint)) * (int)maxCount);
                int devCnt = um_get_device_list(UmxHandle, dstPtr, maxCount);
                if (devCnt > 0)
                {
                    int[] deviceIdArray = new int[devCnt];
                    Marshal.Copy(dstPtr, deviceIdArray, 0, devCnt);
                    for (int i = 0; i < devCnt; i++)
                    {
                        deviceList.Add(deviceIdArray[i]);
                    }
                }
                Marshal.FreeHGlobal(dstPtr);
            }
            return deviceList;
        }

        /// <summary>
        /// Gets the last error code from the device.
        /// </summary>
        /// <returns>The error code from the device, or Constants.LIBUM_ERROR_NOT_OPEN if the device is not open.</returns>
        public int GetLastError()
        {
            int result = Constants.LIBUM_ERROR_NOT_OPEN;
            if (validateState())
            {
                result = um_last_error(UmxHandle);
            }
            return result;
        }

        /// <summary>
        /// Gets the value of a parameter from the device.
        /// </summary>
        /// <param name="paramId">The ID of the parameter to retrieve.</param>
        /// <param name="value">When this method returns, contains the value of the parameter, or int.MaxValue if the device is not open or the parameter does not exist.</param>
        /// <returns>true if the parameter value was retrieved successfully; otherwise, false.</returns>
        public bool GetParameter(int paramId, out int value)
        {
            return GetParameter(DevId, paramId, out value);
        }

        /// <summary>
        /// Gets the value of a parameter from a specific device.
        /// </summary>
        /// <param name="dev">The ID of the device to retrieve the parameter from.</param>
        /// <param name="paramId">The ID of the parameter to retrieve.</param>
        /// <param name="value">When this method returns, contains the value of the parameter, or int.MaxValue if the device is not open or the parameter does not exist.</param>
        /// <returns>true if the parameter value was retrieved successfully; otherwise, false.</returns>
        public bool GetParameter(int dev, int paramId, out int value)
        {
            int v = int.MaxValue;
            int result = Constants.LIBUM_ERROR_NOT_OPEN;
            if (validateState())
            {
                result = um_get_param(UmxHandle, dev, paramId, out v);
            }
            value = v;
            return result >= 0;
        }

        /// <summary>
        /// Determines whether the device is busy.
        /// </summary>
        /// <returns>true if the device is busy; otherwise, false.</returns>
        public bool IsBusy()
        {
            return IsBusy(DevId);
        }

        /// <summary>
        /// Determines whether a specific device is busy.
        /// </summary>
        /// <param name="dev">The ID of the device to check.</param>
        /// <returns>true if the device is busy; otherwise, false.</returns>
        public bool IsBusy(int dev)
        {
            int result = Constants.LIBUM_ERROR_NOT_OPEN;
            if (validateState())
            {
                result = um_is_busy(UmxHandle, dev);
            }
            return result > 0;
        }

        /// <summary>
        /// Pings the device.
        /// </summary>
        /// <returns>true if the device responded to the ping; otherwise, false.</returns>
        public bool Ping()
        {
            return Ping(DevId);
        }

        /// <summary>
        /// Pings a specific device.
        /// </summary>
        /// <param name="dev">The ID of the device to ping.</param>
        /// <returns>true if the device responded to the ping; otherwise, false.</returns>
        public bool Ping(int dev)
        {
            int result = Constants.LIBUM_ERROR_NOT_OPEN;
            if (validateState())
            {
                result = um_ping(UmxHandle, dev);
            }
            return result >= 0;
        }

        /// <summary>
        /// Sets the value of a parameter on the device.
        /// </summary>
        /// <param name="paramId">The ID of the parameter to set.</param>
        /// <param name="value">The value to set the parameter to.</param>
        /// <returns>true if the parameter was set successfully; otherwise, false.</returns>
        public bool SetParameter(int paramId, int value)
        {
            return SetParameter(DevId, paramId, value);
        }

        /// <summary>
        /// Sets the value of a parameter on a specific device.
        /// </summary>
        /// <param name="dev">The ID of the device to set the parameter on.</param>
        /// <param name="paramId">The ID of the parameter to set.</param>
        /// <param name="value">The value to set the parameter to.</param>
        /// <returns>true if the parameter was set successfully; otherwise, false.</returns>
        public bool SetParameter(int dev, int paramId, int value)
        {
            int result = Constants.LIBUM_ERROR_NOT_OPEN;
            if (validateState())
            {
                result = um_set_param(UmxHandle, dev, paramId, value);
            }
            return result >= 0;
        }

        /// <summary>
        /// Stops the specified device.
        /// </summary>
        /// <returns>true if the device was successfully stopped, otherwise false.</returns>
        public bool Stop()
        {
            return Stop(DevId);
        }

        /// <summary>
        /// Stops the specified device.
        /// </summary>
        /// <param name="dev">The device ID to stop.</param>
        /// <returns>true if the device was successfully stopped, otherwise false.</returns>
        public bool Stop(int dev)
        {
            int result = Constants.LIBUM_ERROR_NOT_OPEN;
            if (validateState())
            {
                result = um_stop(UmxHandle, dev);
            }
            return result >= 0;
        }

        /// <summary>
        /// Set options for the next command to be sent to a manipulator.
        /// This is a one-time setting and will be reset after sending the next command.
        /// Can be used to set the trigger for next command (e.g. goto position)
        /// </summary>
        /// <param name="options">
        /// Options bit to set. Use the following flag values:
        /// <list type="table">
        ///     <item>
        ///         <term>SMCP1_OPT_WAIT_TRIGGER_1</term>
        ///         <description>Set command to be run when triggered by physical trigger line2</description>
        ///     </item>
        ///     <item>
        ///         <term>SMCP1_OPT_PRIORITY</term>
        ///         <description>Prioritizes command to run first, 0 = normal command</description>
        ///     </item>
        ///     <item>
        ///         <term>SMCP1_OPT_REQ_BCAST</term>
        ///         <description>Send ACK, RESP or NOTIFY to the bcast address (combine with REQs below), 0 = unicast to the sender</description>
        ///     </item>
        ///     <item>
        ///         <term>SMCP1_OPT_REQ_NOTIFY</term>
        ///         <description>Request notification (e.g. on completed memory drive), 0 = do not notify</description>
        ///     </item>
        ///     <item>
        ///         <term>SMCP1_OPT_REQ_RESP</term>
        ///         <description>Request RESP, 0 = no RESP requested</description>
        ///     </item>
        ///     <item>
        ///         <term>SMCP1_OPT_REQ_ACK</term>
        ///         <description>Request ACK, 0 = no ACK requested</description>
        ///     </item>
        /// </list>
        /// REQ_NOTIFY, REQ_RESP and REQ_ACK are applied automatically for various commands.
        /// This option is applied only to the next command.
        /// Non-zero options are cumulated (bitwise OR'ed).
        /// Call with value zero to reset all options.
        /// </param>
        /// <returns>Returns set optionbits if operation was successful, #um_error otherwise</returns>
        public int CmdOptions(int options)
        {
            return um_cmd_options(UmxHandle, options);
        }

        /// <summary>
        /// Determines whether the current instance of Umx is equal to another object.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance of Umx.</param>
        /// <returns>true if the current instance of Umx is equal to the other object, otherwise false.</returns>
        public override bool Equals(object? obj)
        {
            return obj is Ump umx &&
                   EqualityComparer<um_state>.Default.Equals(UmState, umx.UmState);
        }

        /// <summary>
        /// Returns a hash code for the current instance of Umx.
        /// </summary>
        /// <returns>A hash code for the current instance of Umx.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(UmState);
        }

        /// <summary>
        /// Validates the state of the Umx instance.
        /// </summary>
        /// <returns>true if the state of the Umx instance is valid, otherwise false.</returns>
        protected bool validateState()
        {
            return UmxHandle != IntPtr.Zero;
        }

    }
}
