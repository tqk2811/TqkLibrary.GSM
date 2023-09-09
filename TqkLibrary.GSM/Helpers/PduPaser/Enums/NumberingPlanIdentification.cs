namespace TqkLibrary.GSM.Helpers.PduPaser.Enums
{
    /// <summary>
    /// Numbering plan identification (NPI) byte 0 1 2 3<br>
    /// </br><see href="https://en.wikipedia.org/wiki/GSM_03.40"/>
    /// </summary>
    public enum NumberingPlanIdentification : byte
    {
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown = 0b0000,
        /// <summary>
        /// ISDN/telephone numbering plan (E.164/E.163)
        /// </summary>
        ISDNTelephoneNumberingPlan = 0b0001,
        /// <summary>
        /// Data numbering plan (X.121)
        /// </summary>
        DataNumberingPlan = 0b0011,
        /// <summary>
        /// Telex numbering plan
        /// </summary>
        TelexNumberingPlan = 0b0100,
        /// <summary>
        /// Service Centre Specific plan 1
        /// </summary>
        ServiceCentreSpecificPlan1 = 0b00101,
        /// <summary>
        /// Service Centre Specific plan 2
        /// </summary>
        ServiceCentreSpecificPlan2 = 0b0110,
        /// <summary>
        /// National numbering plan
        /// </summary>
        NationalNumberingPlan = 0b1000,
        /// <summary>
        /// Private numbering plan
        /// </summary>
        PrivateNumberingPlan = 0b1001,
        /// <summary>
        /// ERMES numbering plan (ETSI DE/PS 3 01 3)
        /// </summary>
        ERMESNumberingPlan = 0b1010,
        /// <summary>
        /// Reserved for extension
        /// </summary>
        Reserved = 0b1111
    }
}
