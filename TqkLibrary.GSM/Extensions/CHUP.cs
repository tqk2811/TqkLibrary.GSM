using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.GSM.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public class CommandRequestCHUP : CommandRequest
    {
        internal CommandRequestCHUP(GsmClient gsmClient) : base(gsmClient, "CHUP")
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public new Task<GsmCommandResult> ExecuteAsync(CancellationToken cancellationToken = default)
            => base.ExecuteAsync(cancellationToken);
    }

    /// <summary>
    /// </summary>
    public static class CommandRequestCHUPExtension
    {
        /// <summary>
        /// Voice Hang Up Control
        /// </summary>
        /// <param name="gsmClient"></param>
        /// <returns></returns>
        public static CommandRequestCHUP CHUP(this GsmClient gsmClient) => new CommandRequestCHUP(gsmClient);
    }
}
