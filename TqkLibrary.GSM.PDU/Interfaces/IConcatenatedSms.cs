namespace TqkLibrary.GSM.PDU.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IConcatenatedSms
    {
        /// <summary>
        /// 
        /// </summary>
        ushort CSMSReferenceNumber { get; }
        /// <summary>
        /// 
        /// </summary>
        byte TotalNumberOfParts { get; }
        /// <summary>
        /// 
        /// </summary>
        byte PartNumberInTheSequence { get; }
    }
}
