namespace TqkLibrary.GSM.Helpers.PduPaser.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface ITargetAddressInfo : IAddressInfo
    {
        /// <summary>
        /// 
        /// </summary>
        byte MessageReference { get; }
    }
}
