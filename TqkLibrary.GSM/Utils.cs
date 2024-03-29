﻿namespace TqkLibrary.GSM
{
    /// <summary>
    /// 
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToAtString(this object obj)
            => $"\"{obj}\"";
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="task"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static async Task<TOut> GetTaskResult<TIn, TOut>(this Task<TIn> task, Func<TIn, TOut> expression)
        {
            var result = await task.ConfigureAwait(false);
            return expression.Invoke(result);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static TValue TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
        {
            if (dict.ContainsKey(key)) return dict[key];
            else return default(TValue);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static byte[] HexStringToByteArray(this string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="length"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        public static byte[] ReadToEnd(this Stream ms)
        {
            long length = ms.Length - ms.Position;
            return ms.Read((int)length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="ascii_text"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool StartWith(this byte[] buffer, string ascii_text)
        {
            if (buffer is null) throw new ArgumentNullException(nameof(buffer));
            if (ascii_text is null || ascii_text.Length == 0) throw new ArgumentNullException(nameof(ascii_text));
            if (buffer.Length < ascii_text.Length) return false;

            for (int i = 0; i < ascii_text.Length; i++)
            {
                if (buffer[i] == ascii_text[i]) continue;
                else return false;
            }
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool EndWith(this byte[] buffer, int dataLength, string ascii_text)
        {
            if (buffer is null) throw new ArgumentNullException(nameof(buffer));
            if (ascii_text is null || ascii_text.Length == 0) throw new ArgumentNullException(nameof(ascii_text));
            if (dataLength <= 0 || dataLength > buffer.Length) return false;
            if (dataLength < ascii_text.Length) return false;

            for (int i = ascii_text.Length - 1; i >= 0; i--)
            {
                if (buffer[dataLength - ascii_text.Length + i] == ascii_text[i]) continue;
                else return false;
            }
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hexStringLength"></param>
        /// <returns></returns>
        public static int HexStringLengthToByteLength(this int hexStringLength)
            => (hexStringLength + hexStringLength % 2) / 2;

        /// <summary>
        /// 16 bit checksum based on bitwise XOR<br></br>
        /// If the number of the characters is odd, set the last character as the high 8 bit, and the low 8 bit as 0, and then use an XOR operator to calculate the checksum
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] CheckSum(this byte[] input)
        {
            byte[] result = new byte[2];
            for (int i = 0; i < input.Length; i++)
            {
                if (i % 2 == 0)
                {
                    result[0] ^= input[i];
                }
                else
                {
                    result[1] ^= input[i];
                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chars"></param>
        /// <returns></returns>
        public static string ConvertToString(this IEnumerable<char> chars)
            => new string(chars.ToArray());

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static T RemoveFlag<T>(this T e, T flag) where T : Enum
            => (T)Enum.ToObject(typeof(T), Convert.ToUInt64(e) & (~Convert.ToUInt64(flag)));


#if !NET5_0_OR_GREATER
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> input, int count)
        {
            if (count < 0) throw new IndexOutOfRangeException($"{nameof(count)}: {count} must be >= 0");
            var arr = input is IList<T> ? (IList<T>)input : input.ToArray();
            for (int i = 0; i < arr.Count - count; i++)
            {
                yield return arr[i];
            }
        }
#endif

#if DEBUG
        internal static string PrintCRLFHepler(this string input) => input?.Replace("\r", "\\r").Replace("\n", "\\n");
#endif

        internal static async Task<string> ReadToAsync(this Stream stream, string value, CancellationToken cancellationToken = default)
        {
            byte[] val = Consts.ISO8859.GetBytes(value);
            byte[] result = await ReadToAsync(stream, val, cancellationToken);
            return Consts.ISO8859.GetString(result);
        }
        internal static async Task<byte[]> ReadToAsync(this Stream stream, byte[] value, CancellationToken cancellationToken = default)
        {
            byte[] buffer = new byte[1024];
            bool isLoop = true;
            int offset = 0;
            while (isLoop)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (offset >= buffer.Length)//resize
                {
                    byte[] temp = new byte[buffer.Length + 1024];
                    Array.Copy(buffer, temp, buffer.Length);
                    buffer = temp;
                }

                int byte_read = await stream.ReadAsync(buffer, offset, offset == 0 ? value.Length : 1, cancellationToken);
                offset += byte_read;
                if (offset < value.Length) continue;

                //check last only

                bool isFound = true;
                for (int i = 0; i < value.Length; i++)
                {
                    if (value[i] != buffer[offset - value.Length + i])
                    {
                        isFound = false;
                        break;
                    }
                }
                if (isFound)
                    break;
            }
            return buffer.Take(Math.Max(offset - value.Length, 0)).ToArray();
        }
    }
}
