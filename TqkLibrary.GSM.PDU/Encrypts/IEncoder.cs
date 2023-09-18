namespace TqkLibrary.GSM.PDU.Encrypts
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
