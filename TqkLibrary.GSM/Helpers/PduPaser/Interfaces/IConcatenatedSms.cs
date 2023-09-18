namespace TqkLibrary.GSM.Helpers.PduPaser.Interfaces
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
