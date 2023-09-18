namespace TqkLibrary.GSM.PDU.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDestinationAddress : IAddress
    {
        /// <summary>
        /// 
        /// </summary>
        byte MessageReference { get; }
    }
}
