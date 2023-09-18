namespace TqkLibrary.GSM.PDU.Encrypts
{
    /// <summary>
    /// 
    /// </summary>
    public class SevenBitEncrypt : IEncodeDecode
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="raw"></param>
        /// <param name="dataLength"></param>
        /// <param name="padding"></param>
        /// <returns></returns>
        public string Decode(byte[] raw, int dataLength, int padding = 0)
        {
            var deRaw = decompress(raw, dataLength, padding);
            return ReplaceBasicCharacterSet(Encoding.ASCII.GetString(deRaw));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public byte[] Encode(string str)
        {
            var raw = Encoding.ASCII.GetBytes(str);
            return compress(raw);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="raw"></param>
        /// <param name="dataLength"></param>
        /// <param name="padding"></param>
        /// <returns></returns>
        protected byte[] decompress(byte[] raw, int dataLength, int padding)
        {
            //int realLength = (int)Math.Ceiling(dataLength * 8.0 / 7);
            byte leftBitsLen = 0;
            byte leftBits = 0;
            using (MemoryStream ms = new MemoryStream())
            {
                for (int i = 0; i < raw.Length; i++)
                {
                    byte b = raw[i];
                    if (i == 0 && padding != 0)
                    {
                        leftBits = (byte)(raw[0] >> padding);
                        leftBitsLen = (byte)(8 - padding);
                    }
                    else
                    {
                        byte sevenBits = (byte)(((b << leftBitsLen) | leftBits) & 0x7F);
                        ms.WriteByte(sevenBits);
                        leftBits = (byte)(b >> (7 - leftBitsLen));
                        leftBitsLen++;
                    }
                    if (leftBitsLen == 7)
                    {
                        ms.WriteByte(leftBits);
                        leftBits = 0;
                        leftBitsLen = 0;
                    }
                }
                return ms.ToArray().Take(dataLength).ToArray();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="raw"></param>
        /// <returns></returns>
        protected byte[] compress(byte[] raw)
        {
            int i = 0;
            byte sevenBits = 0;
            byte leftBits = 0;
            byte[] enBytes;
            using (MemoryStream ms = new MemoryStream())
            {
                foreach (var b in raw)
                {
                    sevenBits = (byte)(i & 7);
                    if (sevenBits == 0)
                    {
                        leftBits = b;
                    }
                    else
                    {
                        byte eightBits = (byte)((b << (8 - sevenBits)) | leftBits);
                        ms.WriteByte(eightBits);
                        leftBits = (byte)(b >> sevenBits);
                    }
                    i++;
                }
                ms.WriteByte(leftBits);
                enBytes = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(enBytes, 0, (int)ms.Length);
            }
            return enBytes;
        }



        //https://en.wikipedia.org/wiki/GSM_03.38#cite_ref-3G-TS-23.038_2-0
        /// <summary>
        /// 
        /// </summary>
        public static readonly IReadOnlyDictionary<byte, char> BasicCharacterSet_table = new Dictionary<byte, char>()
        {
            { 0x00, '@' }, { 0x01, '£' }, { 0x02, '$' }, { 0x03, '¥' }, { 0x04, 'è' }, { 0x05, 'é' }, { 0x06, 'ù' }, { 0x07, 'ì' },
            { 0x08, 'ò' }, { 0x09, 'Ç' }, /*{ 0x0a, '\n' },*/ { 0x0b, 'Ø' }, { 0x0c, 'ø' }, /*{ 0x0d, '\r' },*/ { 0x0e, 'Å' }, { 0x0f, 'å' },

            { 0x10, 'Δ' }, { 0x11, '_' }, { 0x12, 'Φ' }, { 0x13, 'Γ' }, { 0x14, 'Λ' }, { 0x15, 'Ω' }, { 0x16, 'Π' }, { 0x17, 'Ψ' },
            { 0x18, 'Σ' }, { 0x19, 'Θ' }, { 0x1a, 'Ξ' }, /*{ 0x1b,ESC },*/ { 0x1c, 'Æ' }, { 0x1d, 'æ' }, { 0x1e, 'ß' }, { 0x1f, 'É' },

            /*{ 0x20, SP },*/ { 0x21, '!' }, { 0x22, '"' }, { 0x23, '#' }, { 0x24, '¤' }, // ASCII

            { 0x40, '¡' },  { 0x5b,'Ä' }, { 0x5c, 'Ö' }, { 0x5d, 'Ñ' }, { 0x5e, 'Ü' }, { 0x5f, '§' }, { 0x60,'¿'},
                            { 0x7b,'ä' }, { 0x7c, 'ö' }, { 0x7d, 'ñ' }, { 0x7e, 'ü' }, { 0x7f, 'à' }
        };
        /// <summary>
        /// 
        /// </summary>
        /// <param name="raw"></param>
        /// <returns></returns>
        public static string ReplaceBasicCharacterSet(string raw)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (char c in raw)
            {
                if (BasicCharacterSet_table.ContainsKey((byte)c))
                    stringBuilder.Append(BasicCharacterSet_table[(byte)c]);
                else
                    stringBuilder.Append(c);
            }
            return stringBuilder.ToString();
        }
    }
}
