/*
Original https://github.com/wi1dcard/sms-decoder
Edit: tqk2811
 */

namespace TqkLibrary.GSM.Helpers.PduPaser
{
    public enum IEI : byte
    {
        ConcatenatedShortMessages = 0x00,
        SpecialSMSMessageIndication = 0x01,
        //Reserved = 0x02,
        //Not used to avoid misinterpretation as <LF> character 0x03
        ApplicationPortAddressingScheme8BitAddress = 0x04,
        ApplicationPortAddressingScheme16BitAddress = 0x05,
        SMSCControlParameters = 0x06,
        UDHSourceIndicator = 0x07,
        ConcatenatedShortMessage16BitReferenceNumber = 0x08,
        WirelessControlMessageProtocol = 0x09,
        TextFormatting = 0x0a,
        PredefinedSound = 0x0b,
        UserDefinedSound = 0x0c,
        PredefinedAnimation = 0x0d,
        LargeAnimation = 0x0e,
        SmallAnimation = 0x0f,
        LargePicture = 0x10,
        SmallPicture = 0x11,
        VariablePicture = 0x12,
        UserPromptIndicator = 0x13,
        ExtendedObject = 0x14,
        ReusedExtendedObject = 0x15,
        CompressionControl = 0x16,
        ObjectDistributionIndicator = 0x17,
        StandardWVGObject = 0x18,
        CharacterSizeWVGObject = 0x19,
        ExtendedObjectDataRequestCommand = 0x1a,
        //Reserved for future EMS features 0x1b->0x1f
        RFC822EMailHeader = 0x20,
        HyperlinkFormatElement = 0x21,
        ReplyAddressElement = 0x22,
        EnhancedVoiceMailInformation = 0x23,
        NationalLanguageSingleShift = 0x24,
        NationalLanguageLockingShift = 0x25,
        //Reserved for future use 0x26 -> 0x6f
        //(U)SIM Toolkit Security Headers 70 – 7F
        //SME to SME specific use 80 – 9F
        //Reserved for future use A0 – BF
        //SC specific use C0 – DF
        //Reserved for future use E0 – FF
    }
}
