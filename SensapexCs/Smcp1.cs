using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensapexCs
{
    public static class Smcpv1Constants
    {
        /// <summary>
        /// Request notification (e.g. on completed memory drive), 0 = do not notify.
        /// </summary>
        public const int SMCP1_OPT_REQ_NOTIFY = 0x00000040;

        /// <summary>
        /// Request RESP, 0 = no RESP requested.
        /// </summary>
        public const int SMCP1_OPT_REQ_RESP = 0x00000020;

        /// <summary>
        /// Request ACK, 0 = no ACK requested.
        /// </summary>
        public const int SMCP1_OPT_REQ_ACK = 0x00000010;

        /// <summary>
        /// Set if frame indicates an error (combine with NOTIFY, RESP or ACK, to be ignored on request).
        /// </summary>
        public const int SMCP1_OPT_ERROR = 0x00000008;

        /// <summary>
        /// Frame is a notification, 0 = not notification.
        /// </summary>
        public const int SMCP1_OPT_NOTIFY = 0x00000004;

        /// <summary>
        /// Frame is an ACK, 0 = not an ACK.
        /// </summary>
        public const int SMCP1_OPT_ACK = 0x00000002;

        /// <summary>
        /// Frame is a request, 0 = frame is a response.
        /// </summary>
        public const int SMCP1_OPT_REQ = 0x00000001;

        /// <summary>
        /// Status bitmask.
        /// </summary>
        public const int SMCP1_STATUS_IDLE = 0x00000000;

        public const int SMCP1_STATUS_X_MOVING = 0x00000010;
        public const int SMCP1_STATUS_Y_MOVING = 0x00000020;
        public const int SMCP1_STATUS_Z_MOVING = 0x00000040;
        public const int SMCP1_STATUS_W_MOVING = 0x00000080;

        /// <summary>
        /// Processing (not necessarily moving).
        /// </summary>
        public const int SMCP1_STATUS_BUSY = 0x00000001;

        /// <summary>
        /// Error state.
        /// </summary>
        public const int SMCP1_STATUS_ERROR = 0x00000008;

    }
}
