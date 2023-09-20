namespace TqkLibrary.GSM.PDU.Enums
{
    /// <summary>
    /// TPDU Types for Mobile Station (MS) and Short Message Service Centre (SMSC, SC).
    /// </summary>
    public enum PduType : byte
    {
        /// <summary>
        /// MS → SC
        /// </summary>
        SmsDeliverReport = 0b00000000,
        /// <summary>
        /// MS → SC
        /// </summary>
        SmsSubmit = 0b00000001,
        /// <summary>
        /// MS → SC
        /// </summary>
        SmsCommand = 0b00000010,


        /// <summary>
        /// SC → MS
        /// </summary>
        SmsDeliver = 0b00000000,
        /// <summary>
        /// SC → MS	
        /// </summary>
        SmsSubmitReport = 0b00000001,
        /// <summary>
        /// SC → MS
        /// </summary>
        SmsStatusReport = 0b00000010,

        /// <summary>
        /// Any
        /// </summary>
        Reserved = 0b00000011,
    }
}
