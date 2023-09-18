namespace TqkLibrary.GSM.PDU.Interfaces
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
