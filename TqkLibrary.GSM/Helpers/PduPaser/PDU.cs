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
        public byte EncodingScheme { get; private set; }
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
                EncodingScheme = (byte)rawPdu.ReadByte();

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
    }
}
