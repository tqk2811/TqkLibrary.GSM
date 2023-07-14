namespace TqkLibrary.GSM.Extended
{
    /// <summary>
    /// 
    /// </summary>
    public class CommandRequestGSN : CommandRequest
    {
        internal CommandRequestGSN(IGsmClient gsmClient) : base(gsmClient, "GSN")
        {

        }

    }
    /// <summary>
    /// 
    /// </summary>
    public static class CommandRequestGSNExtension
    {
        /// <summary>
        /// Serial Number 
        /// </summary>
        /// <param name="gsmClient"></param>
        /// <returns></returns>
        public static CommandRequestGSN GSN(this IGsmClient gsmClient) => new CommandRequestGSN(gsmClient);
    }
}
