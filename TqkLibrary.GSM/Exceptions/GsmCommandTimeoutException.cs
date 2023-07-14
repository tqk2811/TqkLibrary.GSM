namespace TqkLibrary.GSM.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class GsmCommandTimeoutException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public GsmCommandTimeoutException(string message) : base($"Command Timeout \"{message}\"")
        {

        }
    }
}
