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
        public static async Task<bool?> IsPinSimRequired(this GsmClient gsmClient, CancellationToken cancellationToken = default)
        {
            GsmCommandResult gsmCommandResult = await gsmClient.Read("CPIN", cancellationToken).ConfigureAwait(false);
            if ("READY".Equals(gsmCommandResult.Datas.FirstOrDefault())) return true;
            if ("SIM PIN".Equals(gsmCommandResult.Datas.FirstOrDefault())) return false;
            return null;
        }

        public static Task<GsmCommandResult> EnterPinSim(this GsmClient gsmClient, string pin, CancellationToken cancellationToken = default)
            => gsmClient.Write("CPIN", pin, cancellationToken);
    }
}
