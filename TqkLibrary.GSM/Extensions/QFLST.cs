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
    public class CommandRequestQFLST : CommandRequest
    {
        internal CommandRequestQFLST(GsmClient gsmClient) : base(gsmClient, "QFLST")
        {

        }

        /// <summary>
        /// </summary>
        /// <param name="namePattern">
        /// “*” All the files in UFS<br></br>
        /// “RAM:*” All the files in RAM<br></br>
        /// “SD:*” All the files in SD<br></br>
        /// &#8220;&lt;filename&gt;&#8221; The specified file &lt;filename&gt; in UFS<br></br>
        /// &#8220;RAM:&lt;filename&gt;&#8221; The specified file &lt;filename&gt; in RAM<br></br>
        /// &#8220;SD:&lt;filename&gt;&#8221; The specified file &lt;filename&gt; in SD
        /// </param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public new Task<GsmCommandResult> WriteAsync(string namePattern, CancellationToken cancellationToken = default)
            => base.WriteAsync(cancellationToken, namePattern.ToAtString());

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public new Task<GsmCommandResult> ExecuteAsync(CancellationToken cancellationToken = default)
            => base.ExecuteAsync(cancellationToken);
    }

    /// <summary>
    /// </summary>
    public static class CommandRequestQFLSTExtension
    {
        /// <summary>
        /// List Files
        /// </summary>
        /// <param name="gsmClient"></param>
        /// <returns></returns>
        public static CommandRequestQFLST QFLST(this GsmClient gsmClient) => new CommandRequestQFLST(gsmClient);
    }
}
