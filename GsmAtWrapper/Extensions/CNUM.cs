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
        public static Task<string> GetMSISDN(this GsmClient gsmClient, CancellationToken cancellationToken = default)
            => gsmClient.Execute("CNUM", cancellationToken).GetTaskResult(x => x.Datas.FirstOrDefault());
    }
}
