namespace TqkLibrary.GSM.Extended
{
    /// <summary>
    /// 
    /// </summary>
    public class CommandRequestCVHU : CommandRequest
    {
        internal CommandRequestCVHU(IGsmClient gsmClient) : base(gsmClient, "CVHU")
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task<GsmCommandResult> ExecuteAsync(CancellationToken cancellationToken = default)
            => base.ExecuteAsync(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public new async Task<Mode?> ReadAsync(CancellationToken cancellationToken = default)
        {
            var result = await base.ReadAsync(cancellationToken).ConfigureAwait(false);
            var cvhu = result.GetCommandResponse(Command);
            if (result.IsSuccess && cvhu != null && cvhu.Arguments.Count() >= 1)
            {
                if (int.TryParse(cvhu.Arguments.First(), out int mode))
                {
                    return (Mode)mode;
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<GsmCommandResult> WriteAsync(Mode mode, CancellationToken cancellationToken = default)
            => base.WriteAsync(cancellationToken, (int)mode);

        /// <summary>
        /// 
        /// </summary>
        public enum Mode
        {
            /// <summary>
            /// "Drop DTR" ignored but OK result code given. ATH disconnects. 
            /// </summary>
            DropDTRIgnore_And_ATHDisconnect = 0,
            /// <summary>
            /// "Drop DTR" and ATH ignored but OK result code given.
            /// </summary>
            DropDTR_And_ATH_Ignore = 1,
            /// <summary>
            /// "Drop DTR" behaviour according to &amp;D setting. ATH disconnects(factory default). 
            /// </summary>
            DropDTRBehaviourAccordin_And_ATHDisconnect
        }
    }

    /// <summary>
    /// </summary>
    public static class CommandRequestCVHUExtension
    {
        /// <summary>
        /// Voice Hang Up Control
        /// </summary>
        /// <param name="gsmClient"></param>
        /// <returns></returns>
        public static CommandRequestCVHU CVHU(this IGsmClient gsmClient) => new CommandRequestCVHU(gsmClient);
    }
}
