﻿/*
Original https://github.com/wi1dcard/sms-decoder
Edit: tqk2811
 */
using System;
using System.Linq;
using TqkLibrary.GSM.Helpers.PduPaser.Decoders;

namespace TqkLibrary.GSM.Helpers.PduPaser
{
    public class Message
    {
        static readonly SevenBitDecoder sevenBitDecoder = new SevenBitDecoder();
        readonly PDU pdu;
        public Message(PDU pdu)
        {
            this.pdu = pdu ?? throw new ArgumentNullException(nameof(pdu));
        }

        /// <summary>
        /// an address(number) of sms center
        /// </summary>
        public string SmscNumber => GetDecimalSemiOctets(pdu.SmscNumber).TrimEnd('F');

        /// <summary>
        /// an address(number) of sms sender
        /// </summary>
        public string SenderNumber => sevenBitDecoder.Decode(pdu.SenderNumber);

        /// <summary>
        /// timestamp of sms center, this value should be sms sent time of local timezone by default
        /// </summary>
        public DateTime DateTime => ParseDateTime(pdu.TimeStamp);

        /// <summary>
        /// sms content, maybe a part
        /// </summary>
        public string Content => pdu.Decoder?.Decode(pdu.Data, pdu?.UDH?.Padding ?? 0);

        /// <summary>
        /// for long sms which be split, this value will be true
        /// </summary>
        public bool IsSplit => pdu.UDH != null;

        /// <summary>
        /// unique identifier of long sms which be split
        /// </summary>
        public byte? SplitId => pdu.UDH?.CSMSReferenceNumber;

        /// <summary>
        /// number of parts that long sms split into
        /// </summary>
        public byte? SplitCount => pdu.UDH?.TotalNumberOfParts;

        /// <summary>
        /// index of split long sms, when <see cref="isSplit"/> is true
        /// </summary>
        public byte? SplitIndex => pdu.UDH?.PartNumberInTheSequence;


        private static string GetDecimalSemiOctets(byte[] bytes)
        {
            return BitConverter.ToString(bytes.Select(x => (byte)(x >> 4 | x << 4)).ToArray()).Replace("-", string.Empty);
        }

        private static DateTime ParseDateTime(byte[] bytes)
        {
            string time = GetDecimalSemiOctets(bytes);
            byte timeZone = bytes.Last();
            bool timeZoneIsNegative = false;
            if ((timeZone & 0b00001000) == 0b00001000)
            {
                timeZone = (byte)(timeZone & 0b11110111);
                timeZoneIsNegative = true;
            }
            int timeInQuad = int.Parse(new string(timeZone.ToString("x2").Reverse().ToArray()));
            int gmt = timeInQuad * 15 / 60;

            DateTime dateTime = DateTime.ParseExact(
                $"20{time.Substring(0, time.Length - 2)}{(timeZoneIsNegative ? "-" : "+")}{gmt:00}00",
                "yyyyMMddHHmmssK",
                System.Globalization.CultureInfo.CurrentCulture);

            return dateTime;
        }

    }
}