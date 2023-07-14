/*
Original https://github.com/wi1dcard/sms-decoder
Edit: tqk2811
 */

namespace TqkLibrary.GSM.Helpers.PduPaser
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    /// <summary>
    /// https://en.wikipedia.org/wiki/GSM_03.40
    /// </summary>
    [Flags]
    public enum AddressesType : byte
    {
        //TON
        TON_Unknown = 0b10001111,
        InternationalNumber = 0b00010000,
        NationalNumber = 0b00100000,
        NetworkSpecificNumber = 0b00110000,
        SubscriberNumber = 0b01000000,
        Alphanumeric = 0b01010000,
        AbbreviatedNumber = 0b01100000,
        //Reserved for extension 0b01110000

        //NPI
        NPI_Unknown = 0b11110000,
        ISDNTelephoneNumberingPlan = 0b00000001,
        DataNumberingPlan = 0b00000011,
        TelexNumberingPlan = 0b00000100,
        ServiceCentreSpecificPlan1 = 0b00000101,
        ServiceCentreSpecificPlan1_2 = 0b00000110,
        NationalNumberingPlan = 0b00001000,
        PrivateNumberingPlan = 0b00001001,
        ERMESNumberingPlan = 0b00001010,
        //Reserved for extension 0b00001111
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
