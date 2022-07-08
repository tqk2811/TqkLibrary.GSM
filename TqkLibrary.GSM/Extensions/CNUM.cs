using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.GSM.Extensions
{
    public class CommandRequestCNUM : CommandRequest, IExecuteFirstData
    {
        internal CommandRequestCNUM(GsmClient gsmClient) : base(gsmClient, "CNUM")
        {

        }

    }

    public static class CommandRequestCNUMExtension
    {
        /// <summary>
        /// Subscriber Number
        /// </summary>
        /// <param name="gsmClient"></param>
        /// <returns></returns>
        public static CommandRequestCNUM CNUM(this GsmClient gsmClient) => new CommandRequestCNUM(gsmClient);
    }
}
