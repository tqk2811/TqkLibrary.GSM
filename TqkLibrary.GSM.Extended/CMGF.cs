namespace TqkLibrary.GSM.Extended
{
    /// <summary>
    /// 
    /// </summary>
    public class CommandRequestCMGF : CommandRequest
    {
        internal CommandRequestCMGF(IGsmClient gsmClient) : base(gsmClient, "CMGF")
        {

        }

        /// <summary>
        /// 3.5.3.1.3 +CMGF - Message Format
        /// </summary>
        /// <returns></returns>
        public Task<bool> WriteAsync(MessageFormat messageFormat = MessageFormat.TextMode, CancellationToken cancellationToken = default)
            => GsmClient.WriteAsync(Command, ((int)messageFormat).ToString(), cancellationToken).GetTaskResult(x => x.IsSuccess);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public new async Task<MessageFormat?> ReadAsync(CancellationToken cancellationToken = default)
        {
            var result = await base.ReadAsync(cancellationToken).ConfigureAwait(false);
            var cmgf = result.GetCommandResponse(Command);
            if (cmgf != null &&
                cmgf.Arguments.Count() > 0 &&
                int.TryParse(cmgf.Arguments.First(), out int val))
            {
                if (val == (int)MessageFormat.PduMode) return MessageFormat.PduMode;
                if (val == (int)MessageFormat.TextMode) return MessageFormat.TextMode;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public enum MessageFormat
        {
            /// <summary>
            /// 
            /// </summary>
            PduMode = 0,
            /// <summary>
            /// 
            /// </summary>
            TextMode = 1
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public static class CommandRequestCMGFExtension
    {
        /// <summary>
        /// Message Format
        /// </summary>
        /// <param name="gsmClient"></param>
        /// <returns></returns>
        public static CommandRequestCMGF CMGF(this IGsmClient gsmClient) => new CommandRequestCMGF(gsmClient);
    }
}
