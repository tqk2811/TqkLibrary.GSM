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
    public class CommandRequestCIMI : CommandRequest
    {
        internal CommandRequestCIMI(IGsmClient gsmClient) : base(gsmClient, "CIMI")
        {

        }
    }
    /// <summary>
    /// 
    /// </summary>
    public static class CommandRequestCIMIExtension
    {
        /// <summary>
        /// Request International Mobile Subscriber Identity
        /// </summary>
        /// <param name="gsmClient"></param>
        /// <returns></returns>
        public static CommandRequestCIMI CIMI(this IGsmClient gsmClient) => new CommandRequestCIMI(gsmClient);
    }
}
