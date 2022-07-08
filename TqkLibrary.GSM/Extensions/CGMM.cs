using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.GSM.Extensions
{
    public class CommandRequestCGMM : CommandRequest, IExecuteFirstData
    {
        internal CommandRequestCGMM(GsmClient gsmClient) : base(gsmClient, "CGMM")
        {

        }
    }

    public static class CommandRequestCGMMExtension
    {
        /// <summary>
        /// Request Model Identification
        /// </summary>
        /// <param name="gsmClient"></param>
        /// <returns></returns>
        public static CommandRequestCGMM CGMM(this GsmClient gsmClient) => new CommandRequestCGMM(gsmClient);
    }
}
