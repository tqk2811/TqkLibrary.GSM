using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TqkLibrary.GSM
{
    internal static class Consts
    {
        internal const int CommandTimeout
#if DEBUG
            = 20000000;
#else
            = 20000;
#endif
        internal static readonly IReadOnlyDictionary<int, string> CME_Error = new Dictionary<int, string>()
        {
            { 0, "phone failure" },
            { 1, "no connection to phone" },
            { 2, "phone-adaptor link reserved" },
            { 3, "operation not allowed" },
            { 4, "operation not supported" },
            { 5, "PH-SIM PIN required" },
            { 10, "SIM not inserted" },
            { 11, "SIM PIN required" },
            { 12, "SIM PUK required" },
            { 13, "SIM failure" },
            { 14, "SIM busy" },
            { 15, "SIM wrong" },
            { 16, "incorrect password" },
            { 17, "SIM PIN2 required" },
            { 18, "SIM PUK2 required" },
            { 20, "memory full" },
            { 21, "invalid index" },
            { 22, "not found" },
            { 23, "memory failure" },
            { 24, "text string too long" },
            { 25, "invalid characters in text string" },
            { 26, "dial string too long" },
            { 27, "invalid characters in dial string" },
            { 30, "no network service" },
            { 31, "network timeout" },
            { 32, "network not allowed - emergency calls only" },
            { 40, "network personalization PIN required" },
            { 41, "network personalization PUK required" },
            { 42, "network subset personalization PIN required" },
            { 43, "network subset personalization PUK required" },
            { 44, "service provider personalization PIN required" },
            { 45, "service provider personalization PUK required" },
            { 46, "corporate personalization PIN required" },
            { 47, "corporate personalization PUK required" },

            //Easy CAMERA® related errors
            { 50, "Camera not found" },
            { 51, "Camera Initialization Error" },
            { 52, "Camera not Supported" },
            { 53, "No Photo Taken" },
            { 54, "NET BUSY...Camera TimeOut" },
            { 55, "Camera Error" },

            //General purpose error:
            { 100, "unknow" },
            
            //GPRS related errors to a failure to perform an Attach:
            { 103, "Illegal MS (#3)*" },
            { 106, "Illegal ME (#6)*" },
            { 107, "GPRS services not allowed" },
            { 111, "PLMN not allowed" },
            { 112, "Location Area not allowed" },
            { 113, "Roaming not allowed in this location area" },

            //GPRS related errors to a failure to Activate a Context and others
            { 132, "service option not supported" },
            { 133, "requested service option not subscribed" },
            { 134, "service option temporarily out of order" },
            { 148, "unspecified GPRS error" },
            { 149, "PDP authentication failure" },
            { 150, "invalid mobile class" },
            
            //Network survey error
            { 257, "Network survey error (No Carrier)*" },
            { 258, "Network survey error (Busy)*" },
            { 259, "Network survey error (Wrong request)*" },
            { 260, "Network survey error (Aborted)*" },

            //Easy GPRS® related errors
            { 400, "generic undocumented error" },
            { 401, "wrong state" },
            { 402, "wrong mode" },
            { 403, "context already activated" },
            { 404, "stack already active" },
            { 405, "activation failed" },
            { 406, "context not opened" },
            { 407, "cannot setup socket" },
            { 408, "cannot resolve DN" },
            { 409, "timeout in opening socket" },
            { 410, "cannot open socket" },
            { 411, "remote disconnected or timeout" },
            { 412, "connection failed" },
            { 413, "tx error" },
            { 414, "already listening" },

            { 3801, "HTTP time out" },
            { 3802, "HTTP busy" },
            { 3803, "HTTP UART busy" },
            { 3804, "HTTP get no request" },
            { 3805, "HTTP network busy" },
            { 3806, "HTTP network open failed" },
            { 3807, "HTTP network no config" },
            { 3808, "HTTP network deactive" },
            { 3809, "HTTP network error" },
            { 3810, "HTTP url error" },
            { 3811, "HTTP empty url" },
            { 3812, "HTTP ip addr error" },
            { 3813, "HTTP DNS error" },
            { 3814, "HTTP socket create error" },
            { 3815, "HTTP socket connect error" },
            { 3816, "HTTP socket read error" },
            { 3817, "HTTP socket write error" },
            { 3818, "HTTP socket close" },
            { 3819, "HTTP data encode error" },
            { 3820, "HTTP data decode error" },
            { 3821, "HTTP to read timeout" },
            { 3822, "HTTP response failed" },
            { 3823, "incoming call busy" },
            { 3824, "voice call busy" },
            { 3825, "input timeout " },
            { 3826, "wait data timeout" },
            { 3827, "wait http response timeout" },
            { 3828, "alloc memory fail" },
            { 3829, "HTTP need relocation" },

            { 4000, "Exceed max length" },
            { 4001, "Open file fail" },
            { 4002, "Write file fail" },
            { 4003, "Get size fail" },
            { 4004, "Read fail" },
            { 4005, "List file fail" },
            { 4006, "Delete file fail" },
            { 4007, "Get Disk info fail" },
            { 4008, "No space" },
            { 4009, "Time out" },
            { 4010, "File not found" },
            { 4011, "File too large" },
            { 4012, "File already exist" },
            { 4013, "Invalid parameter" },
            { 4014, "Driver error" },
            { 4015, "Create fail" },
            { 4016, "access denied" },
            { 4017, "File too large" },
        };
        internal static readonly IReadOnlyDictionary<int, string> CMS_Error = new Dictionary<int, string>()
        {

        };
        internal const string LineBreak = "\r\n";
        /// <summary>
        /// for not break character >= 0x80 when convert back to byte
        /// </summary>
        internal static readonly Encoding ISO8859 = Encoding.GetEncoding("ISO-8859-1");
    }
}
