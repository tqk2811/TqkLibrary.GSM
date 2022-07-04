using System.Linq;
using System.Text;

namespace TqkLibrary.GSM.Helpers.PduPaser.Decoders
{
    public class UnicodeDecoder : IDecoder
    {
        public string Decode(byte[] raw, int dataLength, int padding = 0)
        {
            return new string(Encoding.BigEndianUnicode.GetString(raw).Take(dataLength / 2).ToArray());
        }

        public byte[] Encode(string str)
        {
            return Encoding.BigEndianUnicode.GetBytes(str);
        }

    }
}
