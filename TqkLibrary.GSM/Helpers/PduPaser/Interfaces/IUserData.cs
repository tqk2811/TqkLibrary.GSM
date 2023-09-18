namespace TqkLibrary.GSM.Helpers.PduPaser.Interfaces
{
    /// <summary>
    /// TP-UD
    /// </summary>
    public interface IUserData
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<byte> GetData();
    }
}
