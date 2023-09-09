/*
Original https://github.com/wi1dcard/sms-decoder
Edit: tqk2811
 */
using TqkLibrary.GSM.Helpers.PduPaser.Interfaces;
using static TqkLibrary.GSM.Helpers.PduPaser.DataCodingScheme;

namespace TqkLibrary.GSM.Helpers.PduPaser
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    /// <summary>
    /// http://www.gsm-modem.de/sms-pdu-mode.html
    /// </summary>
    public class PDU
    {
        private PDU()
        {

        }

        public byte SmscByteLength { get; private set; }
        public byte SmscType { get; private set; }
        public byte[] SmscNumber { get; private set; }

        public PduHeader PduHeader { get; private set; }

        /// <summary>
        /// <see cref="SenderAddressInfo"/> for <see cref="PduType.SmsDeliver"/><br>
        /// </br><see cref="TargetAddressInfo"/> for <see cref="PduType.SmsSubmit"/>
        /// </summary>
        public IAddressInfo AddressInfo { get; private set; }

        public byte ProtocalId { get; private set; }
        public DataCodingScheme DataCodingScheme { get; private set; }
        public IDecoder DataDecoder { get; set; }
        public TimeStampHelper TimeStamp { get; private set; }

        public byte DataLength { get; private set; }
        public byte[] Data { get; private set; }

        public UserDataHeader UDH { get; private set; }


        PDU _Parse(Stream rawPdu)
        {
            rawPdu.Position = 0;

            SmscByteLength = (byte)rawPdu.ReadByte();
            if (SmscByteLength > 0)
            {
                if (SmscByteLength == 1) throw new InvalidDataException("smscByteLength = 1");
                SmscType = (byte)rawPdu.ReadByte();
                SmscNumber = rawPdu.Read(SmscByteLength - 1);
            }

            PduHeader = (PduHeader)rawPdu.ReadByte();
            switch (PduHeader.Type)
            {
                case PduType.SmsDeliver:
                    {
                        AddressInfo = SenderAddressInfo.Parse(rawPdu);

                        ProtocalId = (byte)rawPdu.ReadByte();
                        DataCodingScheme = (DataCodingScheme)rawPdu.ReadByte();

                        switch (DataCodingScheme.CharacterSet)
                        {
                            case DCS_CharacterSet.GSM7Bit:
                                DataDecoder = new SevenBitEncrypt();
                                break;
                            case DCS_CharacterSet.UCS2:
                                DataDecoder = new UnicodeEncrypt();
                                break;

                            default://8bit data or reserved
                                break;
                        }

                        TimeStamp = new TimeStampHelper(rawPdu.Read(7));


                        //length of data in byte
                        //7bit => decode to byte => take DataLength - HeadSize
                        //unicode/8bit => length = DataLength - HeadSize, string_unicode take 1/2 length
                        DataLength = (byte)rawPdu.ReadByte();

                        if (PduHeader.IsUserDataHeaderIndicator)
                        {
                            byte UDH_Length = (byte)rawPdu.ReadByte();
                            byte[] headers = new byte[UDH_Length + 1];
                            headers[0] = UDH_Length;
                            rawPdu.Read(headers, 1, UDH_Length);
                            //https://en.wikipedia.org/wiki/User_Data_Header
                            UDH = new UserDataHeader(headers);
                            Data = rawPdu.ReadToEnd();//length wrong??
                        }
                        else Data = rawPdu.ReadToEnd();//length wrong??
                        break;
                    }
                case PduType.SmsSubmitReport:
                    {

                        break;
                    }

                default: throw new NotSupportedException(PduHeader.Type.ToString());
            }
            return this;
        }

        public static explicit operator byte[](PDU pdu) => pdu.GetBytes().ToArray();
        IEnumerable<byte> GetBytes()
        {
            yield return SmscByteLength;
            if (SmscByteLength > 0)
            {
                if (SmscNumber is null)
                    throw new InvalidDataException($"{nameof(SmscNumber)} is null but {nameof(SmscByteLength)} have value");

                if (SmscByteLength != SmscNumber.Length + 1)
                    throw new InvalidDataException($"{nameof(SmscByteLength)} wrong value");

                yield return SmscType;
                foreach (var b in SmscNumber)
                    yield return b;
            }

            yield return (byte)PduHeader;

            if (AddressInfo is null)
                throw new InvalidDataException($"{nameof(AddressInfo)} is null");
            foreach (var b in AddressInfo.GetData())
                    yield return b;

            yield return ProtocalId;
            yield return (byte)DataCodingScheme;

            if (TimeStamp is null)
                throw new InvalidDataException($"{nameof(TimeStamp)} is null");
            foreach (var b in TimeStamp.GetData())
                yield return b;

            yield return DataLength;
            if (PduHeader.IsUserDataHeaderIndicator)
            {
                foreach (var b in (byte[])UDH)
                    yield return b;
            }
            foreach (var b in Data)
                yield return b;
        }


        public static PDU Parse(byte[] rawPdu)
        {
            using (var ms = new MemoryStream(rawPdu)) return Parse(ms);
        }
        public static PDU Parse(Stream rawPdu) => new PDU()._Parse(rawPdu);


        public static PDU TryParse(byte[] rawPdu)
        {
            using (var ms = new MemoryStream(rawPdu)) return TryParse(ms);
        }
        public static PDU TryParse(Stream rawPdu)
        {
            try
            {
                return new PDU()._Parse(rawPdu);
            }
            catch
            {
                return null;
            }
        }

        //public static PDU Create(
        //    string desNumber, AddressesType desType,

        //    )
        //{
        //    //http://www.gsm-modem.de/sms-pdu-mode.html
        //    PDU pdu = new PDU();
        //    pdu.SmscByteLength = 0;//just zero
        //    pdu.PduHeader = new PduHeader(0x00);
        //    pdu.PduHeader.Type = PduType.SmsSubmit;



        //    return pdu;
        //}
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
