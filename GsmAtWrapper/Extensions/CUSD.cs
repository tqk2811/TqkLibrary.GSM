using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GsmAtWrapper.Extensions
{
    public enum CUSD_N
    {
        Disable = 0,
        Enable = 1,
    }
    public static partial class GsmExtensions
    {
        public static Task<GsmCommandResult> UnstructuredSupplementaryServiceData(
            this GsmClient gsmClient,
            CUSD_N n,
            string str,
            CancellationToken cancellationToken = default)
        {
            return gsmClient.Write("CUSD", cancellationToken, (int)n, $"\"{str}\"");
        }
    }
}
