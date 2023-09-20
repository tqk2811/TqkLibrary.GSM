namespace TqkLibrary.GSM.PDU
{
    /// <summary>
    /// TP-DCS
    /// </summary>
    public class DataCodingScheme
    {
        byte @byte;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="byte"></param>
        public DataCodingScheme(byte @byte)
        {
            this.@byte = @byte;
        }

        //https://en.wikipedia.org/wiki/Data_Coding_Scheme
        //bit 0,1 => class 
        //bit 2,3 => 00 GSM7bit, 01 8bit data, 10 UCS2 (utf16), 11 reserved
        //bit 4 => 0 default, 1 class
        //bit 5,6,7,8 =>    0000 =>     Coding Group: General Data Coding
        //                  0100->0111  Coding Group: Message Marked for Automatic Deletion
        //                  1000->1011  Coding Group: Reserved
        //                  1100        Coding Group: Message Waiting Info: Discard Message
        //                  1101->1110  Coding Group: Message Waiting Info: Store Message
        //                  1111        Coding Group: Data Coding/Message Class

        /// <summary>
        /// Bit 0 &amp; 1
        /// </summary>
        public MessageClass? Class
        {
            get
            {
                if (IsClass)
                {
                    return (MessageClass)(@byte & 0b00000011);
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value is null)
                {
                    IsClass = false;
                }
                else
                {
                    IsClass = true;
                    @byte = (byte)((@byte & 0b11111100) | (byte)value);
                }
            }
        }

        /// <summary>
        /// Bit 2 &amp; 3
        /// </summary>
        public CharacterSet CharacterSet
        {
            get
            {
                return (CharacterSet)((@byte & 0b00001100) >> 2);
            }
            set
            {
                @byte = (byte)((@byte & 0b11110011) | (byte)((byte)value << 2));
            }
        }

        /// <summary>
        /// Bit 4
        /// </summary>
        public bool IsClass
        {
            get => (@byte & 0b00010000) != 0;
            set
            {
                @byte = (byte)((@byte & 0b00010000) | (value ? 0b00000000 : 0b00010000));
            }
        }

        /// <summary>
        /// Bit 5,6,7
        /// </summary>
        public CodingGroup CodingGroup
        {
            get
            {
                switch (@byte)
                {
                    case >= 0x00 and <= 0x3f: return CodingGroup.GeneralDataCoding;
                    case >= 0x40 and <= 0x7f: return CodingGroup.MessageMarkedForAutomaticDeletion;
                    case >= 0x80 and <= 0xbf: return CodingGroup.Reserved;
                    case >= 0xc0 and <= 0xcf: return CodingGroup.MessageWaitingInfo_DiscardMessage;
                    case >= 0xd0 and <= 0xef: return CodingGroup.MessageWaitingInfo_StoreMessage;
                    case >= 0xf0 and <= 0xff: return CodingGroup.DataCoding_Or_MessageClass;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsCompressed
        {
            get
            {
                return (@byte & 0b00100000) != 0;
            }
            set
            {
                @byte = (byte)((@byte & 0b11011111) | (value ? 0b00100000 : 0b00000000));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dcs"></param>
        public static explicit operator byte(DataCodingScheme dcs) => dcs.@byte;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="byte"></param>
        public static implicit operator DataCodingScheme(byte @byte) => new DataCodingScheme(@byte);

    }
}
