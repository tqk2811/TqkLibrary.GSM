using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.GSM.Extensions
{
    public class CommandRequestCCID : CommandRequest, IExecuteFirstData
    {
        internal CommandRequestCCID(GsmClient gsmClient) : base(gsmClient, "CCID")
        {

        }
    }

    public static class CommandRequestCCIDExtension
    {
        /// <summary>
        /// Integrated Circuit Card Identification
        /// </summary>
        /// <returns></returns>
        public static CommandRequestCCID CCID(this GsmClient gsmClient) => new CommandRequestCCID(gsmClient);
    }
}
