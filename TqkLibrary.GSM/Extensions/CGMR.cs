using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.GSM.Extensions
{
    public class CommandRequestCGMR : CommandRequest, IExecuteFirstData
    {
        internal CommandRequestCGMR(GsmClient gsmClient) : base(gsmClient, "CGMR")
        {

        }
    }

    public static class CommandRequestCGMRExtension
    {
        /// <summary>
        /// Request Revision Identification
        /// </summary>
        /// <param name="gsmClient"></param>
        /// <returns></returns>
        public static CommandRequestCGMR CGMR(this GsmClient gsmClient) => new CommandRequestCGMR(gsmClient);
    }
}
