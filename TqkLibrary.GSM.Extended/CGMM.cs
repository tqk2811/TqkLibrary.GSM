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
    public class CommandRequestCGMM : CommandRequest
    {
        internal CommandRequestCGMM(IGsmClient gsmClient) : base(gsmClient, "CGMM")
        {

        }
    }
    /// <summary>
    /// 
    /// </summary>
    public static class CommandRequestCGMMExtension
    {
        /// <summary>
        /// Request Model Identification
        /// </summary>
        /// <param name="gsmClient"></param>
        /// <returns></returns>
        public static CommandRequestCGMM CGMM(this IGsmClient gsmClient) => new CommandRequestCGMM(gsmClient);
    }
}
