using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TqkLibrary.GSM.Extensions;
using TqkLibrary.GSM.Interfaces;

namespace TqkLibrary.GSM
{
    //gsmClient.ABC().Test();
    //gsmClient.ABC().Execute();
    //gsmClient.ABC().Write(obj);
    //gsmClient.ABC().Read();
    /// <summary>
    /// 
    /// </summary>
    public class CommandRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public string Command { get; }
        /// <summary>
        /// 
        /// </summary>
        public IGsmClient GsmClient { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gsmClient"></param>
        /// <param name="command"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public CommandRequest(IGsmClient gsmClient, string command)
        {
            if (string.IsNullOrWhiteSpace(command)) throw new ArgumentNullException(nameof(command));
            this.GsmClient = gsmClient ?? throw new ArgumentNullException(nameof(gsmClient));
            this.Command = command;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public virtual Task<GsmCommandResult> WriteAsync(CancellationToken cancellationToken = default, params object[] values)
            => GsmClient.WriteAsync(Command, cancellationToken, values);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task<GsmCommandResult> WriteAsync(string[] values, CancellationToken cancellationToken = default)
            => GsmClient.WriteAsync(Command, values, cancellationToken);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task<GsmCommandResult> WriteAsync(string value, CancellationToken cancellationToken = default)
            => GsmClient.WriteAsync(Command, value, cancellationToken);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task<GsmCommandResult> ReadAsync(CancellationToken cancellationToken = default)
            => GsmClient.ReadAsync(Command, cancellationToken);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task<GsmCommandResult> ExecuteAsync(CancellationToken cancellationToken = default)
            => GsmClient.ExecuteAsync(Command, cancellationToken);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task<bool> TestAsync(CancellationToken cancellationToken = default)
            => GsmClient.TestAsync(Command, cancellationToken).GetTaskResult(x => x.IsSuccess);
    }
}
