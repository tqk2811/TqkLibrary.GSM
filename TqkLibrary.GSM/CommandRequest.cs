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

        internal protected Task<GsmCommandResult> WriteAsync(CancellationToken cancellationToken = default, params object[] values)
            => GsmClient.WriteAsync(Command, cancellationToken, values);
        internal protected Task<GsmCommandResult> WriteAsync(string[] values, CancellationToken cancellationToken = default)
            => GsmClient.WriteAsync(Command, values, cancellationToken);
        internal protected Task<GsmCommandResult> WriteAsync(string value, CancellationToken cancellationToken = default)
            => GsmClient.WriteAsync(Command, value, cancellationToken);
        internal protected Task<GsmCommandResult> ReadAsync(CancellationToken cancellationToken = default)
            => GsmClient.ReadAsync(Command, cancellationToken);
        internal protected Task<GsmCommandResult> ExecuteAsync(CancellationToken cancellationToken = default)
            => GsmClient.ExecuteAsync(Command, cancellationToken);


        public Task<bool> TestAsync(CancellationToken cancellationToken = default)
            => GsmClient.TestAsync(Command, cancellationToken).GetTaskResult(x => x.IsSuccess);
    }
}
