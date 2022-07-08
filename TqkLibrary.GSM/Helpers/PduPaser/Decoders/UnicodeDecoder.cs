using System.Linq;
using System.Text;
using System;
namespace TqkLibrary.GSM.Helpers.PduPaser.Decoders
{
    public class UnicodeDecoder : IDecoder
    {
        public string Decode(byte[] raw, int dataLength, int padding = 0)
        {
            if(dataLength % 2 != 0) throw new InvalidOperationException($"{nameof(dataLength)} not even");
            return Encoding.BigEndianUnicode.GetString(raw.Take(dataLength).ToArray());
        }

        public byte[] Encode(string str)
        {
            return Encoding.BigEndianUnicode.GetBytes(str);
        }

    }
}
