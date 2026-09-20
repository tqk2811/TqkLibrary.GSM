namespace TqkLibrary.GSM.Extended
{
    /// <summary>
    /// 3GPP TS 27.007 7.2 - Network Registration (circuit switched).
    /// </summary>
    public class CommandRequestCREG : CommandRequestNetworkRegistration
    {
        internal CommandRequestCREG(IGsmClient gsmClient) : base(gsmClient, "CREG")
        {

        }
    }
    /// <summary>
    /// 
    /// </summary>
    public static class CommandRequestCREGExtension
    {
        /// <summary>
        /// Network Registration
        /// </summary>
        /// <param name="gsmClient"></param>
        /// <returns></returns>
        public static CommandRequestCREG CREG(this IGsmClient gsmClient) => new CommandRequestCREG(gsmClient);
    }
}
