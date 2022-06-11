using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GsmAtWrapper
{
    public static class GsmExtensions
    {
        public static Task<bool> SmsTextMode(this GsmClient gsmClient, CancellationToken cancellationToken = default)
            => gsmClient.Write("CMGF", "1", cancellationToken);

        public static Task<string[]> GetManufacturer(this GsmClient gsmClient, CancellationToken cancellationToken = default)
            => gsmClient.Execute("CGMI", cancellationToken);

        public static Task<string[]> GetModel(this GsmClient gsmClient, CancellationToken cancellationToken = default)
            => gsmClient.Execute("CGMM", cancellationToken);

        public static Task<string[]> GetFirmware(this GsmClient gsmClient, CancellationToken cancellationToken = default)
            => gsmClient.Execute("CGMR", cancellationToken);

        public static Task<string[]> GetIMEI(this GsmClient gsmClient, CancellationToken cancellationToken = default)
            => gsmClient.Execute("GSN", cancellationToken);

        public static Task<string[]> GetIMSI(this GsmClient gsmClient, CancellationToken cancellationToken = default)
            => gsmClient.Execute("CIMI", cancellationToken);

        public static Task<string[]> GetICCID(this GsmClient gsmClient, CancellationToken cancellationToken = default)
            => gsmClient.Execute("CCID", cancellationToken);

        public static Task<string[]> GetMSISDN(this GsmClient gsmClient, CancellationToken cancellationToken = default)
            => gsmClient.Execute("CNUM", cancellationToken);

        public static async Task<bool?> IsPinSimRequired(this GsmClient gsmClient, CancellationToken cancellationToken = default)
        {
            var text = await gsmClient.Read("CPIN", cancellationToken).ConfigureAwait(false);
            if ("READY".Equals(text.FirstOrDefault())) return true;
            if ("SIM PIN".Equals(text.FirstOrDefault())) return false;
            return null;
        }

        public static Task<bool> EnterPinSim(this GsmClient gsmClient, string pin, CancellationToken cancellationToken = default)
            => gsmClient.Write("CPIN", pin, cancellationToken);

        /// <summary>
        /// reboot needed
        /// </summary>
        /// <returns></returns>
        public static Task<bool> Remove_SIM_PIN_RequestAtStartup(this GsmClient gsmClient, string pin, CancellationToken cancellationToken = default)
            => gsmClient.Write("CLCK", cancellationToken, "\"SC\"", 0, $"\"{pin}\"");
    }
}
