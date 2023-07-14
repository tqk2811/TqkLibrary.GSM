namespace TqkLibrary.GSM.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAtClient : IBaseGsmClient
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        void Write(string command);
    }
}
