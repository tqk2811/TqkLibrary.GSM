namespace TqkLibrary.GSM.Extended
{
    /// <summary>
    /// 
    /// </summary>
    public class CommandRequestCMEE : CommandRequest
    {
        internal CommandRequestCMEE(IGsmClient gsmClient) : base(gsmClient, "CMEE")
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public new async Task<CMEE_Report?> ReadAsync(CancellationToken cancellationToken = default)
        {
            var result = await base.ReadAsync(cancellationToken).ConfigureAwait(false);
            var cmee = result.GetCommandResponse(Command);
            if (cmee != null &&
                cmee.Arguments.Count() > 0 &&
                int.TryParse(cmee.Arguments.First(), out int val))
            {
                return (CMEE_Report)val;
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="report"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<bool> WriteAsync(CMEE_Report report, CancellationToken cancellationToken = default)
            => GsmClient.WriteAsync(Command, ((int)report).ToString(), cancellationToken).GetTaskResult(x => x.IsSuccess);


        /// <summary>
        /// 
        /// </summary>
        public enum CMEE_Report
        {
            /// <summary>
            /// Disable report, use only ERROR report
            /// </summary>
            Disable = 0,
            /// <summary>
            /// Report numeric format
            /// </summary>
            EnableNumericFormat = 1,
            /// <summary>
            /// Report verbose format
            /// </summary>
            EnableVerboseFormat = 2,
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public static class CommandRequestCMEEExtension
    {
        /// <summary>
        /// Report Mobile Equipment Error 
        /// </summary>
        /// <param name="gsmClient"></param>
        /// <returns></returns>
        public static CommandRequestCMEE CMEE(this IGsmClient gsmClient) => new CommandRequestCMEE(gsmClient);
    }
}
