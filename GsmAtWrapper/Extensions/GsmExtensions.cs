using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GsmAtWrapper.Extensions
{
    public static partial class GsmExtensions
    {
        //public static Task<GsmCommandResult> GetManufacturer(this GsmClient gsmClient, CancellationToken cancellationToken = default)
        //    => gsmClient.Execute("CGMI", cancellationToken);

        //public static Task<GsmCommandResult> GetModel(this GsmClient gsmClient, CancellationToken cancellationToken = default)
        //    => gsmClient.Execute("CGMM", cancellationToken);

        //public static Task<GsmCommandResult> GetFirmware(this GsmClient gsmClient, CancellationToken cancellationToken = default)
        //    => gsmClient.Execute("CGMR", cancellationToken);

        //public static Task<GsmCommandResult> GetIMEI(this GsmClient gsmClient, CancellationToken cancellationToken = default)
        //    => gsmClient.Execute("GSN", cancellationToken);

        //public static Task<GsmCommandResult> GetIMSI(this GsmClient gsmClient, CancellationToken cancellationToken = default)
        //    => gsmClient.Execute("CIMI", cancellationToken);

        //public static Task<GsmCommandResult> GetICCID(this GsmClient gsmClient, CancellationToken cancellationToken = default)
        //    => gsmClient.Execute("CCID", cancellationToken);

        //public static Task<GsmCommandResult> GetMSISDN(this GsmClient gsmClient, CancellationToken cancellationToken = default)
        //    => gsmClient.Execute("CNUM", cancellationToken);

        //public static async Task<bool?> IsPinSimRequired(this GsmClient gsmClient, CancellationToken cancellationToken = default)
        //{
        //    GsmCommandResult gsmCommandResult = await gsmClient.Read("CPIN", cancellationToken).ConfigureAwait(false);
        //    if ("READY".Equals(gsmCommandResult.Datas.FirstOrDefault())) return true;
        //    if ("SIM PIN".Equals(gsmCommandResult.Datas.FirstOrDefault())) return false;
        //    return null;
        //}

        //public static Task<GsmCommandResult> EnterPinSim(this GsmClient gsmClient, string pin, CancellationToken cancellationToken = default)
        //    => gsmClient.Write("CPIN", pin, cancellationToken);

        ///// <summary>
        ///// reboot needed
        ///// </summary>
        ///// <returns></returns>
        //public static Task<GsmCommandResult> Remove_SIM_PIN_RequestAtStartup(this GsmClient gsmClient, string pin, CancellationToken cancellationToken = default)
        //    => gsmClient.Write("CLCK", cancellationToken, "\"SC\"", 0, $"\"{pin}\"");
    }
}
