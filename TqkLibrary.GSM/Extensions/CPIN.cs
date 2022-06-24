using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.GSM.Extensions
{
    public static partial class GsmExtensions
    {
        /// <summary>
        /// 3.5.2.4.3 +CPIN - Enter PIN
        /// </summary>
        /// <returns></returns>
        public static Task<string> ReadEnterPin(this GsmClient gsmClient, CancellationToken cancellationToken = default)
            => gsmClient.Read("CPIN", cancellationToken)
            .GetTaskResult(x => x.CommandResponses.TryGetValue("CPIN")?.Arguments.FirstOrDefault());

        /// <summary>
        /// 3.5.2.4.3 +CPIN - Enter PIN
        /// </summary>
        /// <returns></returns>
        public static Task<GsmCommandResult> WriteEnterPin(this GsmClient gsmClient, string pin, CancellationToken cancellationToken = default)
            => gsmClient.Write("CPIN", cancellationToken, pin.ToAtString());

        /// <summary>
        /// 3.5.2.4.3 +CPIN - Enter PIN
        /// </summary>
        /// <returns></returns>
        public static Task<GsmCommandResult> WriteEnterPin(this GsmClient gsmClient, string pin, string newpin, CancellationToken cancellationToken = default)
            => gsmClient.Write("CPIN", cancellationToken, pin.ToAtString(), newpin.ToAtString());
    }
}
