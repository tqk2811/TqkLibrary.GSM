using System;
using System.Threading;
using System.Threading.Tasks;
using TqkLibrary.GSM.Interfaces;

namespace TqkLibrary.GSM.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class GsmClientExtensions
    {
        /// <summary>
        /// &lt;command&gt;=?
        /// </summary>
        /// <returns></returns>
        public static Task<GsmCommandResult> TestAsync(this IGsmClient gsmClient, string command, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(command)) throw new ArgumentNullException(nameof(command));
            return gsmClient.SendCommandAsync($"AT{command}=?{Consts.LineBreak}", cancellationToken);
        }

        /// <summary>
        /// +&lt;command&gt;?
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Task<GsmCommandResult> ReadAsync(this IGsmClient gsmClient, string command, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(command)) throw new ArgumentNullException(nameof(command));
            return gsmClient.SendCommandAsync($"AT+{command}?{Consts.LineBreak}", cancellationToken);
        }

        /// <summary>
        /// +&lt;command&gt;=[val1],[val2],....
        /// </summary>
        /// <returns></returns>
        public static Task<GsmCommandResult> WriteAsync(this IGsmClient gsmClient, string command, CancellationToken cancellationToken = default, params object[] values)
            => gsmClient.WriteAsync(command, string.Join(",", values), cancellationToken);

        /// <summary>
        /// +&lt;command&gt;=[val1],[val2],....
        /// </summary>
        /// <returns></returns>
        public static Task<GsmCommandResult> WriteAsync(this IGsmClient gsmClient, string command, string[] values, CancellationToken cancellationToken = default)
            => gsmClient.WriteAsync(command, string.Join(",", values), cancellationToken);

        /// <summary>
        /// +&lt;command&gt;=[val1],[val2],....
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Task<GsmCommandResult> WriteAsync(this IGsmClient gsmClient, string command, string value, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(command)) throw new ArgumentNullException(nameof(command));
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(nameof(value));
            return gsmClient.SendCommandAsync($"AT+{command}={value}{Consts.LineBreak}", cancellationToken);
        }

        /// <summary>
        /// +&lt;command&gt;
        /// </summary>
        /// <returns></returns>
        public static Task<GsmCommandResult> ExecuteAsync(this IGsmClient gsmClient, string command, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(command)) throw new ArgumentNullException(nameof(command));
            return gsmClient.SendCommandAsync($"AT+{command}{Consts.LineBreak}", cancellationToken);
        }
    }
}
