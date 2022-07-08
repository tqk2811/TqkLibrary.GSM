using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.GSM
{
    //gsmClient.ABC().Test();
    //gsmClient.ABC().Execute();
    //gsmClient.ABC().Write(obj);
    //gsmClient.ABC().Read();
    public class CommandRequest
    {
        public string Command { get; }
        public GsmClient GsmClient { get; }
        public CommandRequest(GsmClient gsmClient, string command)
        {
            if (string.IsNullOrWhiteSpace(command)) throw new ArgumentNullException(nameof(command));
            this.GsmClient = gsmClient ?? throw new ArgumentNullException(nameof(gsmClient));
            this.Command = command;
        }

        internal protected Task<GsmCommandResult> Write(CancellationToken cancellationToken = default, params object[] values)
            => GsmClient.Write(Command, cancellationToken, values);
        internal protected Task<GsmCommandResult> Write(string[] values, CancellationToken cancellationToken = default)
            => GsmClient.Write(Command, values, cancellationToken);
        internal protected Task<GsmCommandResult> Write(string value, CancellationToken cancellationToken = default)
            => GsmClient.Write(Command, value, cancellationToken);
        internal protected Task<GsmCommandResult> Read(CancellationToken cancellationToken = default)
            => GsmClient.Read(Command, cancellationToken);
        internal protected Task<GsmCommandResult> Execute(CancellationToken cancellationToken = default)
            => GsmClient.Execute(Command, cancellationToken);


        public Task<bool> Test(CancellationToken cancellationToken = default)
            => GsmClient.Test(Command, cancellationToken).GetTaskResult(x => x.IsSuccess);
    }
}
