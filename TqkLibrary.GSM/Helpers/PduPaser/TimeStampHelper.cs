using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TqkLibrary.GSM.Helpers.PduPaser
{
    /// <summary>
    /// 
    /// </summary>
    public class TimeStampHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buff"></param>
        public TimeStampHelper(byte[] buff)
        {
            if (buff is null)
                throw new ArgumentNullException(nameof(buff));
            if (buff.Length != 7)
                throw new InvalidDataException($"{nameof(buff)} array must be length = 7");

            this._buff = buff.ToArray();
        }
        /// <summary>
        /// 
        /// </summary>
        public TimeStampHelper(DateTime dateTime)
        {
            this.TimeStamp = dateTime;
        }

        byte[] _buff = new byte[7];

        /// <summary>
        /// 
        /// </summary>
        public DateTime TimeStamp
        {
            get
            {
                return ParseDateTime(_buff);
            }
            set
            {
                _buff = DateTimeToBuffer(value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<byte> GetData()
        {
            return _buff;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static DateTime ParseDateTime(byte[] bytes)
        {
            string time = bytes.DecimalSemiOctetsToString();
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static byte[] DateTimeToBuffer(DateTime dateTime)
        {
            TimeZoneInfo localZone = TimeZoneInfo.Local;
            TimeSpan timeZone = localZone.BaseUtcOffset;
            bool timeZoneIsNegative = timeZone.Hours < 0;
            int timeInQuad = Math.Abs(timeZone.Hours) * 4; // * 60 / 15

            byte[] buffer = new byte[7];

            byte[] buff_time = dateTime.ToString("yyMMddHHmmss").ToDecimalSemiOctets();
            Array.Copy(buff_time, buffer, 6);

            buffer[6] = (byte)Convert.ToInt32(new string(timeInQuad.ToString().Reverse().ToArray()), 16);
            if(timeZoneIsNegative)
            {
                buffer[6] = (byte)(buffer[6] | 0b00001000);
            }

            return buffer;
        }
    }
}
