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
        public static Task<string> GetModel(this GsmClient gsmClient, CancellationToken cancellationToken = default)
            => gsmClient.Execute("CGMM", cancellationToken).GetTaskResult(x => x.Datas.FirstOrDefault());
        public static Task<bool> TestModel(this GsmClient gsmClient, CancellationToken cancellationToken = default)
            => gsmClient.Test("CGMM", cancellationToken).GetTaskResult(x => x.IsSuccess);
    }
}
