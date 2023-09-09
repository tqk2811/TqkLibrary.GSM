namespace TqkLibrary.GSM.Helpers.PduPaser.Enums
{
    /// <summary>
    /// Type of number (TON) bit 4 5 6<br>
    /// </br><see href="https://en.wikipedia.org/wiki/GSM_03.40"/>
    /// </summary>
    public enum TypeOfNumber : byte
    {
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown = 0b000,
        /// <summary>
        /// International number
        /// </summary>
        InternationalNumber = 0b001,
        /// <summary>
        /// National number
        /// </summary>
        NationalNumber = 0b010,
        /// <summary>
        /// Network specific number
        /// </summary>
        NetworkSpecificNumber = 0b011,
        /// <summary>
        /// Subscriber number
        /// </summary>
        SubscriberNumber = 0b100,
        /// <summary>
        /// Alphanumeric, (coded according to 3GPP TS 23.038 [9] GSM 7-bit default alphabet)
        /// </summary>
        Alphanumeric = 0b101,
        /// <summary>
        /// Abbreviated number
        /// </summary>
        AbbreviatedNumber = 0b110,
        /// <summary>
        /// Reserved for extension
        /// </summary>
        Reserved = 0b111
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
