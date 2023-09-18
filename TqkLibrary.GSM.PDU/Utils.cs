using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TqkLibrary.GSM.PDU
{
    /// <summary>
    /// 
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string DecimalSemiOctetsToString(this IEnumerable<byte> bytes)
            => BitConverter.ToString(bytes.Select(x => (byte)(x >> 4 | x << 4)).ToArray()).Replace("-", string.Empty);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] ToDecimalSemiOctets(this string str)
        {
            if (!str.All(x => x >= '0' && x <= '9'))
                throw new InvalidDataException($"{nameof(str)} only allow number");

            str = (str.Length % 2 == 0) ? str : str + "F";

            return Enumerable
                .Range(0, str.Length / 2)
                .Select(x => Convert.ToInt32(str.Substring(x * 2, 2), 16))
                .Select(x => (byte)(x >> 4 | x << 4))
                .ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        internal static byte[] Read(this Stream ms, int length)
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
        internal static byte[] ReadToEnd(this Stream ms)
        {
            long length = ms.Length - ms.Position;
            return ms.Read((int)length);
        }
    }
}
