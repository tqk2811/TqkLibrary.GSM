using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.GSM.Extensions
{
    public class CommandRequestCPIN : CommandRequest
    {
        internal CommandRequestCPIN(GsmClient gsmClient) : base(gsmClient, "CPIN")
        {

        }


        public new Task<string> Read(CancellationToken cancellationToken = default)
            => base.Read(cancellationToken).GetTaskResult(x => x.GetCommandResponse("CPIN")?.Arguments.FirstOrDefault());

        public new Task<GsmCommandResult> Write(string pin, CancellationToken cancellationToken = default)
            => base.Write(cancellationToken, pin.ToAtString());
        public Task<GsmCommandResult> Write(string pin, string newpin, CancellationToken cancellationToken = default)
            => base.Write(cancellationToken, pin.ToAtString(), newpin.ToAtString());

    }
    public static class CommandRequestCPINExtension
    {
        /// <summary>
        /// Enter PIN 
        /// </summary>
        /// <param name="gsmClient"></param>
        /// <returns></returns>
        public static CommandRequestCPIN CPIN(this GsmClient gsmClient) => new CommandRequestCPIN(gsmClient);
    }
}
