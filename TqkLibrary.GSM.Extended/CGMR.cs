namespace TqkLibrary.GSM.Extended
{
    /// <summary>
    /// 
    /// </summary>
    public class CommandRequestCGMR : CommandRequest
    {
        internal CommandRequestCGMR(IGsmClient gsmClient) : base(gsmClient, "CGMR")
        {

        }
    }
    /// <summary>
    /// 
    /// </summary>
    public static class CommandRequestCGMRExtension
    {
        /// <summary>
        /// Request Revision Identification
        /// </summary>
        /// <param name="gsmClient"></param>
        /// <returns></returns>
        public static CommandRequestCGMR CGMR(this IGsmClient gsmClient) => new CommandRequestCGMR(gsmClient);
    }
}
