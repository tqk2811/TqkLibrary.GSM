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
    public class CommandRequestQFDEL : CommandRequest
    {
        internal CommandRequestQFDEL(IGsmClient gsmClient) : base(gsmClient, "QFDEL")
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task<GsmCommandResult> WriteAsync(string fileName, CancellationToken cancellationToken = default)
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
        public static CommandRequestQFDEL QFDEL(this IGsmClient gsmClient) => new CommandRequestQFDEL(gsmClient);
    }
}
