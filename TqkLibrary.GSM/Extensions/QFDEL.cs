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
    public class CommandRequestQFDEL : CommandRequest
    {
        internal CommandRequestQFDEL(GsmClient gsmClient) : base(gsmClient, "QFDEL")
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public new Task<GsmCommandResult> WriteAsync(string fileName, CancellationToken cancellationToken = default)
            => base.WriteAsync(cancellationToken, fileName.ToAtString());
    }

    /// <summary>
    /// </summary>
    public static class CommandRequestQFDELExtension
    {
        /// <summary>
        /// Delete the File in the Storage
        /// </summary>
        /// <param name="gsmClient"></param>
        /// <returns></returns>
        public static CommandRequestQFDEL QFDEL(this GsmClient gsmClient) => new CommandRequestQFDEL(gsmClient);
    }
}
