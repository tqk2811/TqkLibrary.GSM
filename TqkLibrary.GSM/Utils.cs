using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.IO;

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

        public static byte[] HexStringToByteArray(this string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public static byte[] Read(this Stream ms, int length)
        {
            byte[] reult = new byte[length];
            int byteRead = 0;
            while (byteRead != length)
            {
                byteRead += ms.Read(reult, byteRead, length - byteRead);
            }
            return reult;
        }
        public static byte[] ReadToEnd(this Stream ms)
        {
            long length = ms.Length - ms.Position;
            return ms.Read((int)length);
        }


        public static int HexStringLengthToByteLength(this int hexStringLength)
            => (hexStringLength + hexStringLength % 2) / 2;

#if DEBUG
        internal static string PrintCRLFHepler(this string input) => input?.Replace("\r", "\\r").Replace("\n", "\\n");
#endif
    }
}
