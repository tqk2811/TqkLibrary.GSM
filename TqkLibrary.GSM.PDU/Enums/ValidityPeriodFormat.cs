namespace TqkLibrary.GSM.PDU.Enums
{
    /// <summary>
    /// 9.2.3.3 TP-Validity-Period-Format (TP-VPF)<br>
    /// </br>The TP-Validity-Period-Format is a 2-bit field, located within bit no 3 and 4 of the first octet of <see cref="PduType.SmsSubmit"/>
    /// </summary>
    public enum ValidityPeriodFormat : byte
    {
        /// <summary>
        /// TP-VP field not present
        /// </summary>
        NotPresent = 0b00000000,
        /// <summary>
        /// Reserved
        /// </summary>
        Reserved = 0b00000001,
        /// <summary>
        /// TP-VP field present and integer represented (relative)
        /// </summary>
        PresentAndIntegerRepresented = 0b00000010,
        /// <summary>
        /// TP-VP field present and semi-octet represented (absolute)
        /// </summary>
        PresentAndSemiOctetRepresented = 0b00000011,
    }
}
