﻿namespace TqkLibrary.GSM.PDU
{
    /// <summary>
    /// http://subnets.ru/saved/sms_submit_fo.html <br>
    /// </br>https://en.wikipedia.org/wiki/GSM_03.40
    /// </summary>
    public class PduHeader
    {
        byte _byte;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="byte"></param>
        public PduHeader(byte @byte)
        {
            _byte = @byte;
        }

        /// <summary>
        /// Bit 0 &amp; 1<br>
        /// </br>TP-Message-Type-Indicator (TP-MTI)
        /// </summary>
        public PduType Type//bit 0 & 1
        {
            get
            {
                return (PduType)(_byte & 0b00000011);
            }
            set
            {
                //                      clear value at 0 & 1    set value
                _byte = (byte)((_byte & 0b11111100) | (byte)value);//clear value at 0 & 1
            }
        }

        /// <summary>
        /// Bit 2<br>
        /// </br>TP-More-Messages-to-Send (TP-MMS) in <see cref="PduType.SmsDeliver"/> (0 = more messages)<br>
        /// </br>By setting the TP-More-Messages-to-Send (TP-MMS) bit to 0 (reversed logic), the SMSC signals it has more messages for the recipient (often further segments of a concatenated message). The MSC usually does not close the connection to the mobile phone and does not end the MAP dialogue with the SMSC, which allows faster delivery of subsequent messages or message segments. If by coincidence the further messages vanish from the SMSC in the meantime (when they are for example deleted), the SMSC terminates the MAP dialogue with a MAP Abort message.
        /// </summary>
        public bool IsMoreMessagesToSend
        {
            get { return (_byte & 0b00000100) != 0; }//bit 2
            set
            {
                _byte = (byte)((_byte & 0b11111011) | (value ? 0b00000100 : 0b00000000));
            }
        }

        /// <summary>
        /// Bit 2<br>
        /// </br>TP-Reject-Duplicates (TP-RD) in <see cref="PduType.SmsSubmit"/>
        /// </summary>
        public bool IsRejectDuplicates
        {
            get { return (_byte & 0b00000100) != 0; }//bit 2
            set
            {
                _byte = (byte)((_byte & 0b11111011) | (value ? 0b00000100 : 0b00000000));
            }
        }

        /// <summary>
        /// Bit 3<br>
        /// </br>The TP-Loop-Prevention (TP-LP) bit is designed to prevent looping of <see cref="PduType.SmsDeliver"/> or <see cref="PduType.SmsStatusReport"/> messages routed to a different address than is their destination address or generated by an application. Such message may be sent only if the original message had this flag cleared and the new message must be sent with the flag set.
        /// </summary>
        public bool IsLoopPrevention
        {
            get { return (_byte & 0b00001000) != 0; }//bit 3 and 4
            set
            {
                _byte = (byte)((_byte & 0b11100111) | (value ? 0b00001000 : 0b00000000));
            }
        }

        /// <summary>
        /// Bit 3 &amp; 4<br>
        /// </br>TP-Validity-Period-Format (TP-VPF) in <see cref="PduType.SmsSubmit"/> (00 = not present)
        /// </summary>
        public ValidityPeriodFormat ValidityPeriodFormat
        {
            get => (ValidityPeriodFormat)((_byte & 0b00011000) >> 3);
            set
            {
                _byte = (byte)((_byte & 0b11100111) | (byte)((byte)value << 3));
            }
        }

        /// <summary>
        /// Bit 5<br>
        /// </br>TP-Status-Report-Indication (TP-SRI) in <see cref="PduType.SmsDeliver"/><br>
        /// </br>By setting the TP-Status-Report-Indication (TP-SRI) bit to 1, the SMSC requests a status report to be returned to the SME.
        /// </summary>
        public bool IsStatusReportIndication
        {
            get { return (_byte & 0b00100000) != 0; }//bit 5
            set
            {
                _byte = (byte)((_byte & 0b11011111) | (value ? 0b00100000 : 0b00000000));
            }
        }

        /// <summary>
        /// Bit 5<br>
        /// </br>TP-Status-Report-Request (TP-SRR) in <see cref="PduType.SmsSubmit"/> and <see cref="PduType.SmsCommand"/><br>
        /// </br>By setting the TP-Status-Report-Request (TP-SRR) bit to 1 in a SMS-SUBMIT or SMS-COMMAND, the mobile phone requests a status report to be returned by the SMSC.
        /// </summary>
        public bool IsStatusReportRequest
        {
            get { return (_byte & 0b00100000) != 0; }//bit 5
            set
            {
                _byte = (byte)((_byte & 0b11011111) | (value ? 0b00100000 : 0b00000000));
            }
        }

        /// <summary>
        /// Bit 5<br>
        /// </br>When the TP-Status-Report-Qualifier (TP-SRQ) has value of 1 in an <see cref="PduType.SmsStatusReport"/> message, the message is the result of an <see cref="PduType.SmsCommand"/>; otherwise it is a result of an <see cref="PduType.SmsSubmit"/>.
        /// </summary>
        public bool IsStatusReportQualifier
        {
            get { return (_byte & 0b00100000) != 0; }//bit 5
            set
            {
                _byte = (byte)((_byte & 0b11011111) | (value ? 0b00100000 : 0b00000000));
            }
        }

        /// <summary>
        /// Bit 6<br>
        /// </br>When TP-User-Data-Header-Indicator (TP-UDHI) has value 1, the TP-UD field starts with <see cref="UserDataHeaderIndicator"/>.
        /// </summary>
        public bool IsUserDataHeaderIndicator
        {
            get { return (_byte & 0b01000000) != 0; }//bit 6
            set
            {
                _byte = (byte)((_byte & 0b10111111) | (value ? 0b01000000 : 0b00000000));
            }
        }

        /// <summary>
        /// Bit 7<br>
        /// </br>TP-Reply-Path (TP-RP) in <see cref="PduType.SmsDeliver"/> and <see cref="PduType.SmsSubmit"/><br>
        /// </br>Setting the TP-RP bits turns on a feature which allows to send a reply for a message using the same path as the original message. If the originator and the recipient home networks differ, the reply would go through another SMSC then usually. The mobile operator must take special measures to charge such messages.<br>
        /// </br>Both SM-RP and MAP used to transmit GSM 03.40 TPDU carry enough information to return acknowledgement—the information whether a request was successful or not.However, a GSM 03.40 TPDU may be included in the acknowledgement to carry even more information.The GSM 03.40 has undergone the following development:
        /// </summary>
        public bool IsReplyPath
        {
            get { return (_byte & 0b10000000) != 0; }//bit 7
            set
            {
                _byte = (byte)((_byte & 0b01111111) | (value ? 0b10000000 : 0b00000000));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dcs"></param>
        public static explicit operator byte(PduHeader dcs) => dcs._byte;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="byte"></param>
        public static implicit operator PduHeader(byte @byte) => new PduHeader(@byte);
    }
}