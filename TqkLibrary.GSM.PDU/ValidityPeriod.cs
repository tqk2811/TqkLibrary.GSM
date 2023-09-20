namespace TqkLibrary.GSM.PDU
{
    /// <summary>
    /// 
    /// </summary>
    public class ValidityPeriod
    {
        readonly Pdu _pdu = null;
        byte _b = 0x00;
        /// <summary>
        /// 
        /// </summary>
        public ValidityPeriod(byte b, Pdu pdu)
        {
            _b = b;
            _pdu = pdu ?? throw new ArgumentNullException(nameof(pdu));
        }
        /// <summary>
        /// 
        /// </summary>
        public ValidityPeriod(Pdu pdu) : this(0xff, pdu)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public TimeSpan Time
        {
            get
            {
                byte val = _b;
                switch (_pdu.PduHeader?.ValidityPeriodFormat)
                {
                    case ValidityPeriodFormat.PresentAndIntegerRepresented:
                        break;

                    default:
                        throw new NotSupportedException($"{_pdu.PduHeader?.ValidityPeriodFormat}");
                }
                switch (val)
                {
                    case >= 0 and <= 143:
                        return TimeSpan.FromMinutes((val + 1) * 5);

                    case >= 144 and <= 167:
                        return TimeSpan.FromMinutes(12 * 60 + (val - 143) * 30);

                    case >= 168 and <= 196:
                        return TimeSpan.FromDays(val - 166);

                    case >= 197 and < 255:
                        return TimeSpan.FromDays((val - 192) * 7);

                    case 255://0xff
                        return TimeSpan.Zero;
                }
            }
            set
            {
                int val = 0;
                if (value < TimeSpan.Zero)
                {
                    throw new InvalidDataException($"{nameof(value)} must be larger than Zero");
                }
                else if (value == TimeSpan.Zero)
                {
                    val = 0xff;
                }
                else if (value > TimeSpan.Zero && value <= TimeSpan.FromMinutes(720))//0 -> 12 hr
                {
                    val = (int)Math.Ceiling(value.TotalMinutes / 5) - 1;
                }
                else if (value > TimeSpan.FromMinutes(720) && value <= TimeSpan.FromMinutes(1440))//12hr->24hr
                {
                    val = (int)Math.Ceiling((value.TotalMinutes - 12 * 60) / 30) + 143;
                }
                else if (value > TimeSpan.FromMinutes(1440) && value <= TimeSpan.FromDays(30))// 1day -> 30 days
                {
                    val = (int)Math.Ceiling(value.TotalDays + 166);
                }
                else //if (value > TimeSpan.FromDays(30))//from 30 days -> ...
                {
                    val = Math.Min(254, (int)Math.Ceiling(value.TotalDays / 7 + 192));
                }

                switch (_pdu.PduHeader?.ValidityPeriodFormat)
                {
                    case ValidityPeriodFormat.PresentAndIntegerRepresented:
                        _b = (byte)val;
                        break;

                    default:
                        throw new NotSupportedException($"{_pdu.PduHeader?.ValidityPeriodFormat}");
                }

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<byte> GetData()
        {
            yield return _b;
        }
    }
}
