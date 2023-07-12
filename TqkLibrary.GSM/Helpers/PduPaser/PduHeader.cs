using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TqkLibrary.GSM.Helpers.PduPaser
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    /// <summary>
    /// http://subnets.ru/saved/sms_submit_fo.html <br>
    /// </br>https://en.wikipedia.org/wiki/GSM_03.40
    /// </summary>
    public class PduHeader
    {
        byte _byte;
        public PduHeader(byte @byte)
        {
            this._byte = @byte;
        }

        public PduType Type//bit 0 & 1
        {
            get
            {
                return (PduType)(_byte & 0b00000011);
            }
            set
            {
                //                      clear value at 0 & 1    set value
                this._byte = (byte)((this._byte & 0b11111100) | (byte)value);//clear value at 0 & 1
            }
        }
        public bool IsMoreMessagesToSend
        {
            get { return (_byte & 0b00000100) != 0; }//bit 2
            set
            {
                this._byte = (byte)((this._byte & 0b11111011) | (value ? 0b00000100 : 0b00000000));
            }
        }
        public bool IsLoopPrevention
        {
            get { return (_byte & 0b00001000) != 0; }//bit 3 and 4
            set
            {
                this._byte = (byte)((this._byte & 0b11100111) | (value ? 0b00001000 : 0b00000000));
            }
        }
        public bool IsStatusReportIndication
        {
            get { return (_byte & 0b00100000) != 0; }//bit 5
            set
            {
                this._byte = (byte)((this._byte & 0b11011111) | (value ? 0b00100000 : 0b00000000));
            }
        }
        public bool IsUserDataHeaderIndicator
        {
            get { return (_byte & 0b01000000) != 0; }//bit 6
            set
            {
                this._byte = (byte)((this._byte & 0b10111111) | (value ? 0b01000000 : 0b00000000));
            }
        }
        public bool IsReplyPath
        {
            get { return (_byte & 0b10000000) != 0; }//bit 7
            set
            {
                this._byte = (byte)((this._byte & 0b01111111) | (value ? 0b10000000 : 0b00000000));
            }
        }

        public static explicit operator byte(PduHeader dcs) => dcs._byte;
        public static implicit operator PduHeader(byte @byte) => new PduHeader(@byte);
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
