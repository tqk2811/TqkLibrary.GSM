namespace TqkLibrary.GSM.PDU.Enums
{
    /// <summary>
    /// 
    /// </summary>
    public enum MessageClass : byte
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
}
