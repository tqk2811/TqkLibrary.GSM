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
        public static Task<string> GetManufacturer(this GsmClient gsmClient, CancellationToken cancellationToken = default)
            => gsmClient.Execute("CGMI", cancellationToken).GetTaskResult(x => x.Datas.FirstOrDefault());

        public static Task<bool> TestManufacturer(this GsmClient gsmClient, CancellationToken cancellationToken = default)
            => gsmClient.Test("CGMI", cancellationToken).GetTaskResult(x => x.IsSuccess);
    }
}
