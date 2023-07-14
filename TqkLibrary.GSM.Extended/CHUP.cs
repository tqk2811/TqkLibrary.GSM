namespace TqkLibrary.GSM.Extended
{
    /// <summary>
    /// 
    /// </summary>
    public class CommandRequestCHUP : CommandRequest
    {
        internal CommandRequestCHUP(IGsmClient gsmClient) : base(gsmClient, "CHUP")
        {

        }

        /// <summary>
        /// Execution command cancels all active and held calls, also if a multi-party session is running.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task<GsmCommandResult> ExecuteAsync(CancellationToken cancellationToken = default)
            => base.ExecuteAsync(cancellationToken);
    }

    /// <summary>
    /// </summary>
    public static class CommandRequestCHUPExtension
    {
        /// <summary>
        /// Hang Up Call
        /// </summary>
        /// <param name="gsmClient"></param>
        /// <returns></returns>
        public static CommandRequestCHUP CHUP(this IGsmClient gsmClient) => new CommandRequestCHUP(gsmClient);
    }
}
