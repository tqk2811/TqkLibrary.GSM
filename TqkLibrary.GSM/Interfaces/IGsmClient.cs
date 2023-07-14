using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.GSM.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IGsmClient : IBaseGsmClient
    {
        /// <summary>
        /// 
        /// </summary>
        int CommandTimeout { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<GsmCommandResult> SendCommandAsync(string command, CancellationToken cancellationToken = default);
    }
}
