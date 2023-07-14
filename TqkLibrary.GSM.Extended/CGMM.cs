namespace TqkLibrary.GSM.Extended
{
    /// <summary>
    /// 
    /// </summary>
    public class CommandRequestCGMM : CommandRequest
    {
        internal CommandRequestCGMM(IGsmClient gsmClient) : base(gsmClient, "CGMM")
        {

        }
    }
    /// <summary>
    /// 
    /// </summary>
    public static class CommandRequestCGMMExtension
    {
        /// <summary>
        /// Request Model Identification
        /// </summary>
        /// <param name="gsmClient"></param>
        /// <returns></returns>
        public static CommandRequestCGMM CGMM(this IGsmClient gsmClient) => new CommandRequestCGMM(gsmClient);
    }
}
