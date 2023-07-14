using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TqkLibrary.GSM.Interfaces;

namespace TqkLibrary.GSM.Extended
{
    /// <summary>
    /// 
    /// </summary>
    public class CommandRequestCNUM : CommandRequest
    {
        internal CommandRequestCNUM(IGsmClient gsmClient) : base(gsmClient, "CNUM")
        {

        }

    }
    /// <summary>
    /// 
    /// </summary>
    public static class CommandRequestCNUMExtension
    {
        /// <summary>
        /// Subscriber Number
        /// </summary>
        /// <param name="gsmClient"></param>
        /// <returns></returns>
        public static CommandRequestCNUM CNUM(this IGsmClient gsmClient) => new CommandRequestCNUM(gsmClient);
    }
}
