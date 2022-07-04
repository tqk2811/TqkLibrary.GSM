using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TqkLibrary.GSM.Helpers.PduPaser
{
    public class DataCodingScheme
    {
        readonly byte @byte;
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

        public DCS_Class Class
        {
            get
            {
                if ((@byte & 0b00010000) == 0) return DCS_Class.Default;
                else
                {
                    switch (@byte & 0b00000011)
                    {
                        case 0b00: return DCS_Class.FlashMessage;
                        case 0b01: return DCS_Class.ME_Specific;
                        case 0b10: return DCS_Class.SIM_Or_USIM_Specific;
                        case 0b11: return DCS_Class.TE_Specific;

                        default: return DCS_Class.Default;
                    }
                }
            }
        }
        public DCS_CharacterSet? CharacterSet
        {
            get
            {
                switch ((@byte & 0b00001100) >> 2)
                {
                    case 0b00: return DCS_CharacterSet.GSM7Bit;
                    case 0b01: return DCS_CharacterSet.Data;
                    case 0b10: return DCS_CharacterSet.UCS2;

                    default: return null;
                }
            }
        }

        public DCS_CodingGroup? CodingGroup
        {
            get
            {
                switch (@byte >> 4)
                {
                    case >= 0b0000 and <= 0b0011: return DCS_CodingGroup.GeneralDataCoding;//0x00 -> 0x3f (0b0011 1111)
                    case >= 0b0100 and <= 0b0111: return DCS_CodingGroup.MessageMarkedForAutomaticDeletion;
                    case >= 0b1000 and <= 0b1011: return null;//Reserved
                    case 0b1100: return DCS_CodingGroup.MessageMarkedForAutomaticDeletion;
                    case >= 0b1101 and <= 0b1110: return DCS_CodingGroup.MessageWaitingInfo_StoreMessage;
                    case 0b1111: return DCS_CodingGroup.DataCoding_Or_MessageClass;
                    default: return null;
                }
            }
        }

        public bool IsCompressed
        {

            get
            {
                return (@byte & 0b00100000) != 0;
            }
        }


        public static explicit operator byte(DataCodingScheme dcs) => dcs.@byte;
        public static implicit operator DataCodingScheme(byte @byte) => new DataCodingScheme(@byte);
    }
    public enum DCS_CharacterSet
    {
        GSM7Bit,
        Data,
        UCS2
    }
    public enum DCS_CodingGroup
    {
        GeneralDataCoding,
        MessageMarkedForAutomaticDeletion,
        MessageWaitingInfo_DiscardMessage,
        MessageWaitingInfo_StoreMessage,
        DataCoding_Or_MessageClass,
    }
    public enum DCS_Class
    {
        Default,
        /// <summary>
        /// Class 0
        /// </summary>
        FlashMessage,
        /// <summary>
        /// Class 1
        /// </summary>
        ME_Specific,
        /// <summary>
        /// Class 2
        /// </summary>
        SIM_Or_USIM_Specific,
        /// <summary>
        /// Class 3
        /// </summary>
        TE_Specific
    }
}
