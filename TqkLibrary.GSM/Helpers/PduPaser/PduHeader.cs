using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TqkLibrary.GSM.Helpers.PduPaser
{
    /// <summary>
    /// http://subnets.ru/saved/sms_submit_fo.html <br>
    /// </br>https://en.wikipedia.org/wiki/GSM_03.40
    /// </summary>
    public class PduHeader
    {
        readonly byte @byte;
        public PduHeader(byte @byte)
        {
            this.@byte = @byte;
        }

        public PduType? Type
        {
            get
            {
                switch (@byte & 0b00000011)
                {
                    case 0b00: return PduType.SmsDeliver;
                    case 0b01: return PduType.SmsSubmitReport;
                    case 0b10: return PduType.SmsStatusReport;
                    default: return null;
                }
            }
        }
        public bool IsMoreMessagesToSend
        {
            get { return (@byte & 0b00000100) != 0; }
        }
        public bool IsLoopPrevention
        {
            get { return (@byte & 0b00001000) != 0; }//2bit
        }
        public bool IsStatusReportIndication
        {
            get { return (@byte & 0b00100000) != 0; }
        }
        public bool IsUserDataHeaderIndicator
        {
            get { return (@byte & 0b01000000) != 0; }
        }
        public bool IsReplyPath
        {
            get { return (@byte & 0b10000000) != 0; }
        }

        public static explicit operator byte(PduHeader dcs) => dcs.@byte;
        public static implicit operator PduHeader(byte @byte) => new PduHeader(@byte);
    }

    public enum PduType
    {
        SmsDeliver,
        SmsSubmitReport,
        SmsStatusReport
    }
}
