using TqkLibrary.GSM.PDU.Attributes;
namespace TqkLibrary.GSM.PDU.Enums
{
    /// <summary>
    /// <see href="https://en.wikipedia.org/wiki/User_Data_Header#UDH_Information_Elements">UDH Information Elements</see>
    /// </summary>
    public enum InformationElementIdentifier : byte
    {
        /// <summary>
        /// <see href="https://en.wikipedia.org/wiki/Concatenated_SMS">Concatenated short messages</see>, 8-bit reference number
        /// </summary>
        [IEIClassification(IEIClassification.SmsControl), IEILength(3), IEIMayRepeat(false)] ConcatenatedShortMessages = 0x00,

        /// <summary>
        /// Special SMS Message Indication
        /// </summary>
        [IEIClassification(IEIClassification.SmsControl), IEILength(2), IEIMayRepeat(true)] SpecialSMSMessageIndication = 0x01,

        //Reserved = 0x02,

        //Not used to avoid misinterpretation as <LF> character 0x03

        /// <summary>
        /// <see href="https://en.wikipedia.org/wiki/Smart_Messaging">Application port addressing scheme</see>, 8 bit address
        /// </summary>
        [IEIClassification(IEIClassification.SmsControl), IEILength(2), IEIMayRepeat(false)] ApplicationPortAddressingScheme8BitAddress = 0x04,

        /// <summary>
        /// <see href="https://en.wikipedia.org/wiki/Smart_Messaging">Application port addressing scheme</see>, 16 bit address
        /// </summary>
        [IEIClassification(IEIClassification.SmsControl), IEILength(4), IEIMayRepeat(false)] ApplicationPortAddressingScheme16BitAddress = 0x05,

        /// <summary>
        /// SMSC Control Parameters
        /// </summary>
        [IEIClassification(IEIClassification.SmsControl), IEILength(1), IEIMayRepeat(false)] SMSCControlParameters = 0x06,

        /// <summary>
        /// UDH Source Indicator
        /// </summary>
        [IEIClassification(IEIClassification.SmsControl), IEILength(1), IEIMayRepeat(true)] UDHSourceIndicator = 0x07,

        /// <summary>
        /// <see href="https://en.wikipedia.org/wiki/Concatenated_SMS">Concatenated short messages</see>, 16-bit reference number
        /// </summary>
        [IEIClassification(IEIClassification.SmsControl), IEILength(4), IEIMayRepeat(false)] ConcatenatedShortMessage16BitReferenceNumber = 0x08,

        /// <summary>
        /// <see href="https://en.wikipedia.org/wiki/Wireless_Application_Protocol">Wireless Control Message Protocol</see>
        /// </summary>
        [IEIClassification(IEIClassification.SmsControl), IEILength(1, 255), IEIMayRepeat(true)] WirelessControlMessageProtocol = 0x09,

        /// <summary>
        /// Text Formatting
        /// </summary>
        [IEIClassification(IEIClassification.EmsControl), IEILength(3, 4), IEIMayRepeat(true)] TextFormatting = 0x0a,

        /// <summary>
        /// Predefined Sound
        /// </summary>
        [IEIClassification(IEIClassification.EmsContent), IEILength(2), IEIMayRepeat(true)] PredefinedSound = 0x0b,

        /// <summary>
        /// User Defined Sound (<see href="https://en.wikipedia.org/wiki/IMelody">iMelody</see> max 128 bytes)
        /// </summary>
        [IEIClassification(IEIClassification.EmsContent), IEILength(2, 129), IEIMayRepeat(true)] UserDefinedSound = 0x0c,

        /// <summary>
        /// Predefined Animation
        /// </summary>
        [IEIClassification(IEIClassification.EmsContent), IEILength(2), IEIMayRepeat(true)] PredefinedAnimation = 0x0d,

        /// <summary>
        /// Large Animation (16*16 times 4 = 32*4 =128 bytes)
        /// </summary>
        [IEIClassification(IEIClassification.EmsContent), IEILength(129), IEIMayRepeat(true)] LargeAnimation = 0x0e,

        /// <summary>
        /// Small Animation (8*8 times 4 = 8*4 =32 bytes)
        /// </summary>
        [IEIClassification(IEIClassification.EmsContent), IEILength(33), IEIMayRepeat(true)] SmallAnimation = 0x0f,

        /// <summary>
        /// Large Picture (32*32 = 128 bytes)
        /// </summary>
        [IEIClassification(IEIClassification.EmsContent), IEILength(129), IEIMayRepeat(true)] LargePicture = 0x10,

        /// <summary>
        /// Small Picture (16*16 = 32 bytes)
        /// </summary>
        [IEIClassification(IEIClassification.EmsContent), IEILength(33), IEIMayRepeat(true)] SmallPicture = 0x11,

        /// <summary>
        /// Variable Picture
        /// </summary>
        [IEIClassification(IEIClassification.EmsContent), IEILength(4, 255), IEIMayRepeat(true)] VariablePicture = 0x12,

        /// <summary>
        /// User prompt indicator
        /// </summary>
        [IEIClassification(IEIClassification.EmsControl), IEILength(1), IEIMayRepeat(true)] UserPromptIndicator = 0x13,

        /// <summary>
        /// Extended Object
        /// </summary>
        [IEIClassification(IEIClassification.EmsContent), IEILength(7, 255), IEIMayRepeat(true)] ExtendedObject = 0x14,

        /// <summary>
        /// Reused Extended Object
        /// </summary>
        [IEIClassification(IEIClassification.EmsControl), IEILength(3), IEIMayRepeat(true)] ReusedExtendedObject = 0x15,

        /// <summary>
        /// Compression Control
        /// </summary>
        [IEIClassification(IEIClassification.EmsControl), IEILength(3, 255), IEIMayRepeat(false)] CompressionControl = 0x16,

        /// <summary>
        /// Object Distribution Indicator
        /// </summary>
        [IEIClassification(IEIClassification.EmsControl), IEILength(2), IEIMayRepeat(true)] ObjectDistributionIndicator = 0x17,

        /// <summary>
        /// Standard WVG object
        /// </summary>
        [IEIClassification(IEIClassification.EmsContent), IEILength(1, 255), IEIMayRepeat(true)] StandardWVGObject = 0x18,

        /// <summary>
        /// Character Size WVG object
        /// </summary>
        [IEIClassification(IEIClassification.EmsContent), IEILength(1, 255), IEIMayRepeat(true)] CharacterSizeWVGObject = 0x19,

        /// <summary>
        /// Extended Object Data Request Command
        /// </summary>
        [IEIClassification(IEIClassification.EmsControl), IEILength(0, 255), IEIMayRepeat(false)] ExtendedObjectDataRequestCommand = 0x1a,

        //Reserved for future EMS features 0x1b->0x1f

        /// <summary>
        /// RFC 822 E-Mail Header
        /// </summary>
        [IEIClassification(IEIClassification.SmsControl), IEILength(1), IEIMayRepeat(false)] RFC822EMailHeader = 0x20,

        /// <summary>
        /// Hyperlink format element
        /// </summary>
        [IEIClassification(IEIClassification.SmsControl), IEILength(0, 255), IEIMayRepeat(true)] HyperlinkFormatElement = 0x21,

        /// <summary>
        /// Reply Address Element
        /// </summary>
        [IEIClassification(IEIClassification.SmsControl), IEILength(1, 255), IEIMayRepeat(false)] ReplyAddressElement = 0x22,

        /// <summary>
        /// Enhanced Voice Mail Information
        /// </summary>
        [IEIClassification(IEIClassification.SmsControl), IEILength(0, 255), IEIMayRepeat(false)] EnhancedVoiceMailInformation = 0x23,

        /// <summary>
        /// National Language Single Shift
        /// </summary>
        [IEIClassification(IEIClassification.SmsControl), IEILength(1), IEIMayRepeat(false)] NationalLanguageSingleShift = 0x24,

        /// <summary>
        /// National Language Locking Shift
        /// </summary>
        [IEIClassification(IEIClassification.SmsControl), IEILength(1), IEIMayRepeat(false)] NationalLanguageLockingShift = 0x25,


        //Reserved for future use 0x26 -> 0x6f
        //(U)SIM Toolkit Security Headers 70 – 7F
        //SME to SME specific use 80 – 9F
        //Reserved for future use A0 – BF
        //SC specific use C0 – DF
        //Reserved for future use E0 – FF
    }
}
