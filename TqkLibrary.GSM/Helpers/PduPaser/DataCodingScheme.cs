namespace TqkLibrary.GSM.Helpers.PduPaser
{
    /// <summary>
    /// 
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
        public DCS_MessageClass? Class
        {
            get
            {
                if (IsClass)
                {
                    return (DCS_MessageClass)(@byte & 0b00000011);
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
                    @byte = (byte)((@byte & 0b11111100) | (byte)value);
                }
            }
        }

        /// <summary>
        /// Bit 2 &amp; 3
        /// </summary>
        public DCS_CharacterSet CharacterSet
        {
            get
            {
                return (DCS_CharacterSet)((@byte & 0b00001100) >> 2);
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
        public DCS_CodingGroup CodingGroup
        {
            get
            {
                switch (@byte)
                {
                    case >= 0x00 and <= 0x3f: return DCS_CodingGroup.GeneralDataCoding;
                    case >= 0x40 and <= 0x7f: return DCS_CodingGroup.MessageMarkedForAutomaticDeletion;
                    case >= 0x80 and <= 0xbf: return DCS_CodingGroup.Reserved;
                    case >= 0xc0 and <= 0xcf: return DCS_CodingGroup.MessageWaitingInfo_DiscardMessage;
                    case >= 0xd0 and <= 0xef: return DCS_CodingGroup.MessageWaitingInfo_StoreMessage;
                    case >= 0xf0 and <= 0xff: return DCS_CodingGroup.DataCoding_Or_MessageClass;
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

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public enum DCS_CharacterSet
        {
            GSM7Bit = 0b00,
            Data = 0b01,
            UCS2 = 0b10,
            Reserved = 0b11,
        }
        public enum DCS_CodingGroup
        {
            GeneralDataCoding,
            MessageMarkedForAutomaticDeletion,
            MessageWaitingInfo_DiscardMessage,
            MessageWaitingInfo_StoreMessage,
            DataCoding_Or_MessageClass,
            Reserved
        }
        public enum DCS_MessageClass : byte
        {
            /// <summary>
            /// Class 0
            /// </summary>
            FlashMessage = 0b00,
            /// <summary>
            /// Class 1
            /// </summary>
            ME_Specific = 0b01,
            /// <summary>
            /// Class 2
            /// </summary>
            SIM_Or_USIM_Specific = 0b10,
            /// <summary>
            /// Class 3
            /// </summary>
            TE_Specific = 0b11,
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
