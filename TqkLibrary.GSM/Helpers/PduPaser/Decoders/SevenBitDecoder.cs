using System.IO;
using System.Text;

namespace TqkLibrary.GSM.Helpers.PduPaser.Decoders
{
    public class SevenBitDecoder : IDecoder
    {
        public string Decode(byte[] raw, int padding = 0)
        {
            var deRaw = this.decompress(raw, padding);
            return Encoding.ASCII.GetString(deRaw);
        }

        public byte[] Encode(string str)
        {
            var raw = Encoding.ASCII.GetBytes(str);
            return this.compress(raw);
        }

        protected byte[] decompress(byte[] raw, int padding)
        {
            byte[] deBytes;
            byte leftBitsLen = 0;
            byte leftBits = 0;
            byte sevenBits = 0;
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
                        sevenBits = (byte)(((b << leftBitsLen) | leftBits) & 0x7F);
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
                ms.Position = 0;
                deBytes = new byte[ms.Length];
                ms.Read(deBytes, 0, (int)ms.Length);
            }
            return deBytes;
        }

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
    }
}
