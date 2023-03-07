using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.GSM.Extensions
{
    public interface IExecuteFirstData
    {
    }

    public static class ExecuteFirstDataExtension
    {
        public static Task<string> Execute<T>(this T t, CancellationToken cancellationToken = default) where T : CommandRequest, IExecuteFirstData
            => t.ExecuteAsync(cancellationToken).GetTaskResult(x => x.Datas.FirstOrDefault());
    }
}
