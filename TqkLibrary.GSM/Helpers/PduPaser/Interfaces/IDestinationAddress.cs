namespace TqkLibrary.GSM.Helpers.PduPaser.Interfaces
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
