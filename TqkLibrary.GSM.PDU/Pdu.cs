/*
Original https://github.com/wi1dcard/sms-decoder
Edit: tqk2811
 */
using System.IO;
using TqkLibrary.GSM.PDU.Enums;
using TqkLibrary.GSM.PDU.Encrypts;
using TqkLibrary.GSM.PDU.Interfaces;
using TqkLibrary.GSM.PDU.UserDataHeaderIndicatorDatas;
using static TqkLibrary.GSM.PDU.DataCodingScheme;

namespace TqkLibrary.GSM.PDU
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    /// <summary>
    /// http://www.gsm-modem.de/sms-pdu-mode.html
    /// </summary>
    public class Pdu
    {
        private Pdu()
        {

        }

        public byte SmscByteLength { get; private set; }
        public byte SmscType { get; private set; }
        public byte[] SmscNumber { get; private set; }

        public PduHeader PduHeader { get; private set; }

        /// <summary>
        /// <see cref="OriginatingAddress"/> for <see cref="PduType.SmsDeliver"/><br>
        /// </br><see cref="DestinationAddress"/> for <see cref="PduType.SmsSubmit"/>
        /// </summary>
        public IAddress Address { get; private set; }

        /// <summary>
        /// TP-PID
        /// </summary>
        public byte ProtocalIdentifier { get; private set; }
        /// <summary>
        /// TP-DCS
        /// </summary>
        public DataCodingScheme DataCodingScheme { get; private set; }
        /// <summary>
        /// TP-SCTS or TP-VP with <see cref="ValidityPeriodFormat.PresentAndSemiOctetRepresented"/>
        /// </summary>
        public ServiceCentreTimeStamp ServiceCentreTimeStamp { get; private set; }
        /// <summary>
        /// TP-VP
        /// </summary>
        public ValidityPeriod ValidityPeriod { get; private set; }
        public UserDataHeaderIndicator UserDataHeaderIndicator { get; private set; }
        public IDecoder DataDecoder { get; set; }
        public byte DataLength { get; private set; }
        public byte[] Data { get; private set; }



        Pdu _Parse(Stream rawPdu)
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
                        Address = OriginatingAddress.Parse(rawPdu);

                        ProtocalIdentifier = (byte)rawPdu.ReadByte();
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

                        ServiceCentreTimeStamp = new ServiceCentreTimeStamp(rawPdu.Read(7));


                        //length of data in byte
                        //7bit => decode to byte => take DataLength - HeadSize
                        //unicode/8bit => length = DataLength - HeadSize, string_unicode take 1/2 length
                        DataLength = (byte)rawPdu.ReadByte();

                        if (PduHeader.IsUserDataHeaderIndicator)
                        {
                            UserDataHeaderIndicator = UserDataHeaderIndicator.Read(rawPdu);
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

        public static explicit operator byte[](Pdu pdu) => pdu.GetBytes().ToArray();
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

            if (Address is null)
                throw new InvalidDataException($"{nameof(Address)} is null");
            foreach (var b in Address.GetData())
                yield return b;

            yield return ProtocalIdentifier;
            yield return (byte)DataCodingScheme;

            switch (PduHeader.ValidityPeriodFormat)
            {
                case ValidityPeriodFormat.NotPresent:
                case ValidityPeriodFormat.PresentAndSemiOctetRepresented:
                    if (ServiceCentreTimeStamp is null)
                        throw new InvalidDataException($"{nameof(this.ServiceCentreTimeStamp)} is null");
                    foreach (var b in ServiceCentreTimeStamp.GetData())
                        yield return b;
                    break;

                case ValidityPeriodFormat.PresentAndIntegerRepresented:
                    if (ValidityPeriod is null)
                        throw new InvalidDataException($"{nameof(this.ValidityPeriod)} is null");
                    foreach (var b in ValidityPeriod.GetData())
                        yield return b;
                    break;
            }

            yield return DataLength;
            if (PduHeader.IsUserDataHeaderIndicator)
            {
                foreach (var b in (byte[])UserDataHeaderIndicator)
                    yield return b;
            }
            foreach (var b in Data)
                yield return b;
        }


        public static Pdu Parse(byte[] rawPdu)
        {
            using (var ms = new MemoryStream(rawPdu)) return Parse(ms);
        }
        public static Pdu Parse(Stream rawPdu) => new Pdu()._Parse(rawPdu);


        public static Pdu TryParse(byte[] rawPdu)
        {
            using (var ms = new MemoryStream(rawPdu)) return TryParse(ms);
        }
        public static Pdu TryParse(Stream rawPdu)
        {
            try
            {
                return new Pdu()._Parse(rawPdu);
            }
            catch
            {
                return null;
            }
        }

        public static IEnumerable<Pdu> Create(
            string desNumber, string message, TimeSpan? ValidityPeriod = null
            )
        {
            if (!ValidityPeriod.HasValue)
                ValidityPeriod = TimeSpan.Zero;

            bool isNumber = desNumber.All(x => x == '+' || (x >= '0' && x <= '9'));
            bool isUnicode = message.Any(x => x > 128);

            IEncodeDecode encodeDecode = isUnicode ? new UnicodeEncrypt() : new SevenBitEncrypt();
            Encoding encoding = isUnicode ? Encoding.Unicode : Encoding.ASCII;
            int chars_length_per_msg = isUnicode ? 76 : 160;//ASCII 160, unicode 70

            List<string> msgs = Enumerable.Range(0, (int)Math.Ceiling(message.Length * 1.0 / chars_length_per_msg))
                .Select(x => message.Substring(x * chars_length_per_msg, Math.Min(chars_length_per_msg, message.Length - x * chars_length_per_msg)))
                .ToList();
            byte CSMSReferenceNumber = (byte)new Random((int)DateTime.Now.Ticks).Next();

            for (int i = 0; i < msgs.Count; i++)
            {
                //http://www.gsm-modem.de/sms-pdu-mode.html
                //https://www.smsdeliverer.com/online-sms-pdu-encoder.aspx
                Pdu pdu = new Pdu();
                pdu.SmscByteLength = 0;//just zero
                pdu.PduHeader = new PduHeader(0x00);
                pdu.PduHeader.Type = PduType.SmsSubmit;
                pdu.PduHeader.IsStatusReportRequest = false;//if need report success or failed
                pdu.PduHeader.ValidityPeriodFormat = ValidityPeriodFormat.PresentAndIntegerRepresented;
                pdu.PduHeader.IsUserDataHeaderIndicator = msgs.Count > 1;
                pdu.Address = new DestinationAddress()
                {
                    MessageReference = 0,
                    AddressLength = (byte)desNumber.TrimStart('+').Length,
                    //If a subscriber enters a telephone number with `+' sign at its start, the `+' sign will be removed and the address gets TON=1 (international number), NPI=1.
                    //The number itself must always start with a country code and must be formatted exactly according to the E.164 standard.
                    NPI = isNumber ? NumberingPlanIdentification.ISDNTelephoneNumberingPlan : NumberingPlanIdentification.Unknown,
                    TON = isNumber ? TypeOfNumber.Unknown : TypeOfNumber.Alphanumeric,
                    Address = isNumber ? desNumber.Trim('+').ToDecimalSemiOctets() : new SevenBitEncrypt().Encode(desNumber),
                };
                pdu.ProtocalIdentifier = 0;
                pdu.DataCodingScheme = new DataCodingScheme(0x00);
                //pdu.DataCodingScheme.Class = DCS_MessageClass.FlashMessage;
                pdu.DataCodingScheme.CharacterSet = isUnicode ? DCS_CharacterSet.UCS2 : DCS_CharacterSet.GSM7Bit;

                pdu.ValidityPeriod = new ValidityPeriod(pdu);
                pdu.ServiceCentreTimeStamp = new ServiceCentreTimeStamp(DateTime.Now.AddHours(1));

                if (pdu.PduHeader.IsUserDataHeaderIndicator)
                {
                    pdu.UserDataHeaderIndicator = new UserDataHeaderIndicator();
                    pdu.UserDataHeaderIndicator.InformationElementIdentifier = InformationElementIdentifier.ConcatenatedShortMessages;
                    pdu.UserDataHeaderIndicator.UserData = new ConcatenatedSms8()
                    {
                        CSMSReferenceNumber = CSMSReferenceNumber,
                        TotalNumberOfParts = (byte)msgs.Count,
                        PartNumberInTheSequence = (byte)(i + 1),
                    };
                    pdu.DataLength = (byte)(msgs[i].Length + ((byte[])pdu.UserDataHeaderIndicator).Length);
                }
                else
                {
                    pdu.DataLength = (byte)msgs[i].Length;
                }
                pdu.Data = encodeDecode.Encode(msgs[i]);

#if DEBUG
                //https://www.smsdeliverer.com/online-sms-pdu-encoder.aspx Validity period (hours):	 2
                //test 1
                //0011000A81 3018111111 00 00 17 17 C8329BFD064D9B5362999DB697E565B96BFC6E8700
                //test 2
                //0051000A81 3018111111 00 08 17 8C 050003000301 005200FA00740020006C007500690020002C0020006B006800F4006E006700200072006100200074006800EA006D002000731EA3006E0020007000681EA9006D0020006E00EA006E0020006300F30020007400681EDD00690020006700690061006E0020006300680061007500200063006800751ED10074002000630068006F002000621EA3
                //0051000A81 3018111111 00 08 17 8C 050003000302 006E00200074006800E2006E002C0020006E00EA006E0020006E006800EC006E0020006E006701B01EDD00690020006300F30020007400ED0020006301A1002000721ED300690020002C00200074006100790020006E00E000790020006300681EAF00630020006D1ED90074002000761EE300740020006C01690020007200751ED300690020
                //0051000A81 3018111111 00 08 17 48 050003000303 00621EC700700020006400ED0020002C0020006300F2006E0020007200751ED3006900200063006800610020006300681EAF00630020006D1ED900740020007600E3

                //tool
                //test 1
                //0011000A01 3018111111 00 00 32901112008282 17 C8329BFD064D9B5362999DB697E565B96BFC6E8700
                //test 2
                //                          time -  data length - udh - dataaaaa
                //0051000A01 3018111111 00 08 32901112111482 46 050003970300 005200FA00740020006C007500690020002C0020006B006800F4006E006700200072006100200074006800EA006D002000731EA3006E0020007000681EA9006D0020006E00EA006E0020006300F30020007400681EDD00690020006700690061006E0020006300680061007500200063006800751ED10074002000630068006F002000621EA3006E00200074
                //0051000A01 3018111111 00 08 32901112316082 46 050003100301 006800E2006E002C0020006E00EA006E0020006E006800EC006E0020006E006701B01EDD00690020006300F30020007400ED0020006301A1002000721ED300690020002C00200074006100790020006E00E000790020006300681EAF00630020006D1ED90074002000761EE300740020006C01690020007200751ED30069002000621EC700700020006400ED
                //0051000A01 3018111111 00 08 32901112316082 1B 050003100302 0020002C0020006300F2006E0020007200751ED3006900200063006800610020006300681EAF00630020006D1ED900740020007600E3
                string hex = BitConverter.ToString(pdu.GetBytes().ToArray()).Replace("-", string.Empty);
#endif
                yield return pdu;
            }
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
