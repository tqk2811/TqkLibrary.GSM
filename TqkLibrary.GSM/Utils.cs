using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
namespace TqkLibrary.GSM
{
    public static class Utils
    {
        public static string ToAtString(this object obj)
            => $"\"{obj}\"";

        public static async Task<TOut> GetTaskResult<TIn, TOut>(this Task<TIn> task, Func<TIn, TOut> expression)
        {
            var result = await task.ConfigureAwait(false);
            return expression.Invoke(result);
        }

        public static TValue TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
        {
            if (dict.ContainsKey(key)) return dict[key];
            else return default(TValue);
        }
    }
}
