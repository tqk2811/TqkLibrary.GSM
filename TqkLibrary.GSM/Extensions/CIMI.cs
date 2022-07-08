using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.GSM.Extensions
{
    public class CommandRequestCIMI : CommandRequest, IExecuteFirstData
    {
        internal CommandRequestCIMI(GsmClient gsmClient) : base(gsmClient, "CIMI")
        {

        }
    }

    public static class CommandRequestCIMIExtension
    {
        /// <summary>
        /// Request International Mobile Subscriber Identity
        /// </summary>
        /// <param name="gsmClient"></param>
        /// <returns></returns>
        public static CommandRequestCIMI CIMI(this GsmClient gsmClient) => new CommandRequestCIMI(gsmClient);
    }
}
