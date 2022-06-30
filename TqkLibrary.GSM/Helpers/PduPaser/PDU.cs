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

        public TP PduHeader { get; private set; }

        public byte SenderLength { get; private set; }
        public byte SenderType { get; private set; }
        public byte[] SenderNumber { get; private set; }

        public byte ProtocalId { get; private set; }
        public byte DataCodingScheme { get; private set; }
        public IDecoder Decoder { get; set; }
        public byte[] TimeStamp { get; private set; }
        public byte DataLength { get; private set; }


        public byte[] Data { get; private set; }
        public UserDataHeader UDH { get; private set; }
        public bool IsIncludedDataHeader => PduHeader.HasFlag(TP.UDHI);

        byte SenderByteLength => (byte)((SenderLength + SenderLength % 2) / 2);

        //SC -> MS  (short message service centre -> Mobile Station)
        //https://en.wikipedia.org/wiki/GSM_03.40
        public const TP SMS_DELIVER = TP.None;
        public const TP SMS_SUBMIT_REPORT = TP.MTI;
        public const TP SMS_STATUS_REPORT = TP.MTI2;


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

            PduHeader = (TP)rawPdu.ReadByte();
            if ((PduHeader & SMS_DELIVER) == SMS_DELIVER)
            {
                SenderLength = (byte)rawPdu.ReadByte();
                SenderType = (byte)rawPdu.ReadByte();
                SenderNumber = rawPdu.Read(SenderByteLength);

                ProtocalId = (byte)rawPdu.ReadByte();
                DataCodingScheme = (byte)rawPdu.ReadByte();
                //https://en.wikipedia.org/wiki/Data_Coding_Scheme
                //bit 0,1 => class 
                //bit 2,3 => 00 GSM7bit, 01 8bit data, 10 UCS2 (utf16), 11 reserved
                //bit 5 => 0 default, 1 class
                //bit 5,6,7,8 =>    0000 => ^
                //                  0100->0111  Coding Group: Message Marked for Automatic Deletion
                //                  1000->1011  Coding Group: Reserved
                //                  1100        Coding Group: Message Waiting Info: Discard Message
                //                  1101->1110  Coding Group: Message Waiting Info: Store Message
                //                  1111        Coding Group: Data Coding/Message Class
                switch ((DataCodingScheme & 0b00001100) >> 2)//bit 2,3
                {
                    case 0b00:
                        Decoder = new SevenBitDecoder();
                        break;
                    case 0b10:
                        Decoder = new UnicodeDecoder();
                        break;

                    default://8bit data or reserved
                        break;
                }

                TimeStamp = rawPdu.Read(7);

                DataLength = (byte)rawPdu.ReadByte();
                if (IsIncludedDataHeader)
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
            }
            else if ((PduHeader & SMS_SUBMIT_REPORT) == SMS_SUBMIT_REPORT)
            {
                throw new NotSupportedException();
            }
            else if ((PduHeader & SMS_STATUS_REPORT) == SMS_STATUS_REPORT)
            {
                throw new NotSupportedException();
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
