/*
Original https://github.com/wi1dcard/sms-decoder
Edit: tqk2811
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TqkLibrary.GSM.Helpers.PduPaser.Decoders;

namespace TqkLibrary.GSM.Helpers.PduPaser
{
    public class PDU
    {
        private PDU()
        {

        }

        public byte SmscByteLength { get; private set; }
        public byte SmscType { get; private set; }
        public byte[] SmscNumber { get; private set; }

        public PduHeader PduHeader { get; private set; }

        public byte SenderLength { get; private set; }
        public AddressesType SenderType { get; private set; }
        public byte[] SenderNumber { get; private set; }

        public byte ProtocalId { get; private set; }
        public DataCodingScheme DataCodingScheme { get; private set; }
        public IDecoder DataDecoder { get; set; }
        public byte[] TimeStamp { get; private set; }
        public byte DataLength { get; private set; }


        public byte[] Data { get; private set; }
        public UserDataHeader UDH { get; private set; }

        byte SenderByteLength => (byte)((SenderLength + SenderLength % 2) / 2);

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
                        SenderLength = (byte)rawPdu.ReadByte();
                        SenderType = (AddressesType)rawPdu.ReadByte();
                        SenderNumber = rawPdu.Read(SenderByteLength);

                        ProtocalId = (byte)rawPdu.ReadByte();
                        DataCodingScheme = (DataCodingScheme)rawPdu.ReadByte();

                        switch (DataCodingScheme.CharacterSet)
                        {
                            case DCS_CharacterSet.GSM7Bit:
                                DataDecoder = new SevenBitDecoder();
                                break;
                            case DCS_CharacterSet.UCS2:
                                DataDecoder = new UnicodeDecoder();
                                break;

                            default://8bit data or reserved
                                break;
                        }

                        TimeStamp = rawPdu.Read(7);


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

                default: throw new NotSupportedException(PduHeader.Type?.ToString());
            }
            return this;
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
    }
}
