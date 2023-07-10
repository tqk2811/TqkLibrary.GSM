using System.Linq;
using System.Text;
using System;
namespace TqkLibrary.GSM.Helpers.PduPaser.Encrypts
{
    /// <summary>
    /// 
    /// </summary>
    public class UnicodeDecoder : IDecoder, IEncoder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="raw"></param>
        /// <param name="dataLength"></param>
        /// <param name="padding"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public string Decode(byte[] raw, int dataLength, int padding = 0)
        {
            if (dataLength % 2 != 0) throw new InvalidOperationException($"{nameof(dataLength)} not even");
            return Encoding.BigEndianUnicode.GetString(raw.Take(dataLength).ToArray());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public byte[] Encode(string str)
        {
            return Encoding.BigEndianUnicode.GetBytes(str);
        }

    }
}
