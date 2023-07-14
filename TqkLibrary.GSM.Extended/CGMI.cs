namespace TqkLibrary.GSM.Extended
{
    /// <summary>
    /// 
    /// </summary>
    public class CommandRequestCGMI : CommandRequest
    {
        internal CommandRequestCGMI(IGsmClient gsmClient) : base(gsmClient, "CGMI")
        {

        }
    }
    /// <summary>
    /// 
    /// </summary>
    public static class CommandRequestCGMIExtension
    {
        /// <summary>
        /// Request Manufacturer Identification
        /// </summary>
        /// <param name="gsmClient"></param>
        /// <returns></returns>
        public static CommandRequestCGMI CGMI(this IGsmClient gsmClient) => new CommandRequestCGMI(gsmClient);
    }
}
