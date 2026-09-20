namespace TqkLibrary.GSM.Extended
{
    /// <summary>
    /// 3GPP TS 27.007 10.1.19 - GPRS Network Registration Status.
    /// </summary>
    public class CommandRequestCGREG : CommandRequestNetworkRegistration
    {
        internal CommandRequestCGREG(IGsmClient gsmClient) : base(gsmClient, "CGREG")
        {

        }
    }
    /// <summary>
    /// 
    /// </summary>
    public static class CommandRequestCGREGExtension
    {
        /// <summary>
        /// GPRS Network Registration Status
        /// </summary>
        /// <param name="gsmClient"></param>
        /// <returns></returns>
        public static CommandRequestCGREG CGREG(this IGsmClient gsmClient) => new CommandRequestCGREG(gsmClient);
    }
}
