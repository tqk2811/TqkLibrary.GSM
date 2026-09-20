namespace TqkLibrary.GSM.Extended
{
    /// <summary>
    /// 3GPP TS 27.007 10.1.20 - EPS Network Registration Status (LTE).
    /// </summary>
    public class CommandRequestCEREG : CommandRequestNetworkRegistration
    {
        internal CommandRequestCEREG(IGsmClient gsmClient) : base(gsmClient, "CEREG")
        {

        }
    }
    /// <summary>
    /// 
    /// </summary>
    public static class CommandRequestCEREGExtension
    {
        /// <summary>
        /// EPS Network Registration Status
        /// </summary>
        /// <param name="gsmClient"></param>
        /// <returns></returns>
        public static CommandRequestCEREG CEREG(this IGsmClient gsmClient) => new CommandRequestCEREG(gsmClient);
    }
}
