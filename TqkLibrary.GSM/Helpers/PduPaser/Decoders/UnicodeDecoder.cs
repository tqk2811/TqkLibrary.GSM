using System.Text;

namespace TqkLibrary.GSM.Helpers.PduPaser.Decoders
{
    public class UnicodeDecoder : IDecoder
    {
        public string Decode(byte[] raw, int padding = 0)
        {
            return Encoding.BigEndianUnicode.GetString(raw);
        }

        public byte[] Encode(string str)
        {
            return Encoding.BigEndianUnicode.GetBytes(str);
        }

    }
}
