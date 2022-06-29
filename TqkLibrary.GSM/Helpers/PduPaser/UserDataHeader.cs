/*
Original https://github.com/wi1dcard/sms-decoder
Edit: tqk2811
 */
using System.Linq;

namespace TqkLibrary.GSM.Helpers.PduPaser
{
    public class UserDataHeader
    {
        readonly byte[] Data;
        internal UserDataHeader(byte[] data)
        {
            this.Data = data;
        }
        public byte UDHLength => Data.FirstOrDefault();
        public IEI IEI => (IEI)Data.Skip(1).FirstOrDefault();
        public byte IELength => Data.Skip(2).FirstOrDefault();
        public byte CSMSReferenceNumber => Data.Skip(3).FirstOrDefault();
        public byte TotalNumberOfParts => Data.Skip(4).FirstOrDefault();
        public byte PartNumberInTheSequence => Data.Skip(5).FirstOrDefault();

        //https://en.wikipedia.org/wiki/User_Data_Header
        //https://en.wikipedia.org/wiki/Concatenated_SMS
        public int Padding => (7 - ((8 * (1 + UDHLength)) % 7)) % 7;
    }
}
