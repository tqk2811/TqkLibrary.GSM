/*
Original https://github.com/wi1dcard/sms-decoder
Edit: tqk2811
 */

namespace TqkLibrary.GSM.Helpers.PduPaser
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class UserDataHeader
    {
        readonly byte[] Data;
        internal UserDataHeader(byte[] data)
        {
            Data = data;
        }
        public int HeaderLength => Data.Length;
        public byte UDHLength => Data.FirstOrDefault();
        public IEI IEI => (IEI)Data.Skip(1).FirstOrDefault();
        public byte IELength => Data.Skip(2).FirstOrDefault();
        public byte CSMSReferenceNumber => Data.Skip(3).FirstOrDefault();
        public byte TotalNumberOfParts => Data.Skip(4).FirstOrDefault();
        public byte PartNumberInTheSequence => Data.Skip(5).FirstOrDefault();

        //https://en.wikipedia.org/wiki/User_Data_Header
        //https://en.wikipedia.org/wiki/Concatenated_SMS
        public int Padding => (7 - ((8 * (1 + UDHLength)) % 7)) % 7;

        public static explicit operator byte[](UserDataHeader userData) => userData.Data.ToArray();
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
