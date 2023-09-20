namespace TqkLibrary.GSM.PDU.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDecoder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="raw"></param>
        /// <param name="dataLength"></param>
        /// <param name="padding"></param>
        /// <returns></returns>
        string Decode(byte[] raw, int dataLength, int padding = 0);
    }
}
