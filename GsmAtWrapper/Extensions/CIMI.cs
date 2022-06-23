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
        public static Task<string> GetIMSI(this GsmClient gsmClient, CancellationToken cancellationToken = default)
            => gsmClient.Execute("CIMI", cancellationToken).GetTaskResult(x => x.Datas.FirstOrDefault());

        public static Task<bool> TestIMSI(this GsmClient gsmClient, CancellationToken cancellationToken = default)
            => gsmClient.Test("CIMI", cancellationToken).GetTaskResult(x => x.IsSuccess);
    }
}
