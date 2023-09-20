namespace TqkLibrary.GSM.PDU.Encrypts
{
    /// <summary>
    /// 
    /// </summary>
    public class UnicodeEncrypt : IEncodeDecode
    {
        /// <summary>
        /// 
        /// </summary>
        public static UnicodeEncrypt Instance { get; } = new UnicodeEncrypt();

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
