using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.GSM.Extensions
{
    public class CommandRequestCMGF : CommandRequest
    {
        internal CommandRequestCMGF(GsmClient gsmClient) : base(gsmClient, "CMGF")
        {

        }

        /// <summary>
        /// 3.5.3.1.3 +CMGF - Message Format
        /// </summary>
        /// <param name="gsmClient"></param>
        /// <param name="messageFormat"></param>
        /// <returns></returns>
        public Task<bool> Write(MessageFormat messageFormat = MessageFormat.TextMode)
            => GsmClient.Write(Command, ((int)messageFormat).ToString()).GetTaskResult(x => x.IsSuccess);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public new async Task<MessageFormat?> Read(CancellationToken cancellationToken = default)
        {
            var result = await base.Read(cancellationToken).ConfigureAwait(false);
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
    }

    public static class CommandRequestCMGFExtension
    {
        /// <summary>
        /// Message Format
        /// </summary>
        /// <param name="gsmClient"></param>
        /// <returns></returns>
        public static CommandRequestCMGF CMGF(this GsmClient gsmClient) => new CommandRequestCMGF(gsmClient);
    }
    public enum MessageFormat
    {
        PduMode = 0,
        TextMode = 1
    }
}
