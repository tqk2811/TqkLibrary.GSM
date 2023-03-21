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
    public class CommandRequestQFDWL : CommandRequest
    {
        internal CommandRequestQFDWL(GsmClient gsmClient) : base(gsmClient, "QFDWL")
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
    public static class CommandRequestQFDWLExtension
    {
        /// <summary>
        /// Download the File from the Storage
        /// </summary>
        /// <param name="gsmClient"></param>
        /// <returns></returns>
        public static CommandRequestQFDWL QFDWL(this GsmClient gsmClient) => new CommandRequestQFDWL(gsmClient);
    }
}
