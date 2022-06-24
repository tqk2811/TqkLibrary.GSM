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
        public static Task<string> GetFirmware(this GsmClient gsmClient, CancellationToken cancellationToken = default)
            => gsmClient.Execute("CGMR", cancellationToken).GetTaskResult(x => x.Datas.FirstOrDefault());

        public static Task<bool> TestFirmware(this GsmClient gsmClient, CancellationToken cancellationToken = default)
            => gsmClient.Test("CGMR", cancellationToken).GetTaskResult(x => x.IsSuccess);
    }
}
