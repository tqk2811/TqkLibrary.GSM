/*
Original https://github.com/wi1dcard/sms-decoder
Edit: tqk2811
 */
namespace TqkLibrary.GSM.PDU
{
    /// <summary>
    /// 
    /// </summary>
    public class Message
    {
        static readonly SevenBitEncrypt sevenBitDecoder = new SevenBitEncrypt();
        readonly Pdu pdu;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pdu"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public Message(Pdu pdu)
        {
            this.pdu = pdu ?? throw new ArgumentNullException(nameof(pdu));
        }

        /// <summary>
        /// an address(number) of sms center
        /// </summary>
        public string SmscNumber => pdu.SmscNumber.DecimalSemiOctetsToString().TrimEnd('F');

        /// <summary>
        /// an address(number) of sms sender
        /// </summary>
        public string SenderNumber
        {
            get
            {
                if (pdu.Address is not null &&
                    pdu.Address is OriginatingAddress senderAddressInfo)
                {
                    if (senderAddressInfo.NPI.HasFlag(NumberingPlanIdentification.ISDNTelephoneNumberingPlan))
                    {
                        return senderAddressInfo.Address.DecimalSemiOctetsToString().TrimEnd('F');
                    }
                    else if (senderAddressInfo.TON.HasFlag(TypeOfNumber.Alphanumeric))
                    {
                        return sevenBitDecoder.Decode(senderAddressInfo.Address.ToArray(), senderAddressInfo.AddressLength);
                    }
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// timestamp of sms center, this value should be sms sent time of local timezone by default
        /// </summary>
        public DateTime DateTime => pdu.ServiceCentreTimeStamp.TimeStamp;

        /// <summary>
        /// sms content, maybe a part
        /// </summary>
        public string Content => pdu.DataDecoder?.Decode(
            pdu.Data.ToArray(),
            (int)pdu.UserDataLength - (pdu.UserDataHeaderIndicator?.GetData()?.Count() ?? 0),
            pdu?.UserDataHeaderIndicator?.Padding ?? 0);

        /// <summary>
        /// for long sms which be split, this value will be true
        /// </summary>
        public bool IsSplit =>
            pdu.UserDataHeaderIndicator?.InformationElementIdentifier == InformationElementIdentifier.ConcatenatedShortMessages ||
            pdu.UserDataHeaderIndicator?.InformationElementIdentifier == InformationElementIdentifier.ConcatenatedShortMessage16BitReferenceNumber;

        /// <summary>
        /// unique identifier of long sms which be split
        /// </summary>
        public UInt16? SplitId
        {
            get
            {
                if (pdu?.UserDataHeaderIndicator?.UserData is IConcatenatedSms concatenatedSms)
                {
                    return concatenatedSms.CSMSReferenceNumber;
                }
                return null;
            }
        }

        /// <summary>
        /// number of parts that long sms split into
        /// </summary>
        public byte? SplitCount
        {
            get
            {
                if (pdu?.UserDataHeaderIndicator?.UserData is IConcatenatedSms concatenatedSms)
                {
                    return concatenatedSms.TotalNumberOfParts;
                }
                return null;
            }
        }

        /// <summary>
        /// index of split long sms, when <see cref="IsSplit"/> is true
        /// </summary>
        public byte? SplitIndex
        {
            get
            {
                if (pdu?.UserDataHeaderIndicator?.UserData is IConcatenatedSms concatenatedSms)
                {
                    return concatenatedSms.PartNumberInTheSequence;
                }
                return null;
            }
        }
    }
}
