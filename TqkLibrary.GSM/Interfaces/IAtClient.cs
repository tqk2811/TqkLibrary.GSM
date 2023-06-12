namespace TqkLibrary.GSM.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAtClient : IGsmClient
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        void Write(string command);
    }
}
