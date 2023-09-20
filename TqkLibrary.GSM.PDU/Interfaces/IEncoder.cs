namespace TqkLibrary.GSM.PDU.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IEncoder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        byte[] Encode(string str);
    }
}
