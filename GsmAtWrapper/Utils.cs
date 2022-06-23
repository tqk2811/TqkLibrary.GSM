using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
namespace GsmAtWrapper
{
    public static class Utils
    {
        public static string ToAtString(this object obj)
            => $"\"{obj}\"";

        public static async Task<TOut> GetTaskResult<TIn,TOut>(this Task<TIn> task, Func<TIn,TOut> expression)
        {
            var result = await task.ConfigureAwait(false);
            return expression.Invoke(result);
        }
    }
}
