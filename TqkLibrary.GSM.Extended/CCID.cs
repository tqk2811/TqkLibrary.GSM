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
    public class CommandRequestCCID : CommandRequest
    {
        internal CommandRequestCCID(IGsmClient gsmClient) : base(gsmClient, "CCID")
        {

        }
    }
    /// <summary>
    /// 
    /// </summary>
    public static class CommandRequestCCIDExtension
    {
        /// <summary>
        /// Integrated Circuit Card Identification
        /// </summary>
        /// <returns></returns>
        public static CommandRequestCCID CCID(this IGsmClient gsmClient) => new CommandRequestCCID(gsmClient);
    }
}
