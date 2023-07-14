using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.GSM.Extended
{
    /// <summary>
    /// 
    /// </summary>
    public class CommandRequestCPIN : CommandRequest
    {
        internal CommandRequestCPIN(IGsmClient gsmClient) : base(gsmClient, "CPIN")
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public new Task<string> ReadAsync(CancellationToken cancellationToken = default)
            => base.ReadAsync(cancellationToken).GetTaskResult(x => x.GetCommandResponse("CPIN")?.Arguments.FirstOrDefault());
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pin"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task<GsmCommandResult> WriteAsync(string pin, CancellationToken cancellationToken = default)
            => base.WriteAsync(cancellationToken, pin.ToAtString());
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pin"></param>
        /// <param name="newpin"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<GsmCommandResult> WriteAsync(string pin, string newpin, CancellationToken cancellationToken = default)
            => base.WriteAsync(cancellationToken, pin.ToAtString(), newpin.ToAtString());

    }
    /// <summary>
    /// 
    /// </summary>
    public static class CommandRequestCPINExtension
    {
        /// <summary>
        /// Enter PIN 
        /// </summary>
        /// <param name="gsmClient"></param>
        /// <returns></returns>
        public static CommandRequestCPIN CPIN(this IGsmClient gsmClient) => new CommandRequestCPIN(gsmClient);
    }
}
