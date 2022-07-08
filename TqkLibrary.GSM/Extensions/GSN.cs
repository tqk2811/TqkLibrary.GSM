using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.GSM.Extensions
{
    public class CommandRequestGSN : CommandRequest, IExecuteFirstData
    {
        internal CommandRequestGSN(GsmClient gsmClient) : base(gsmClient, "GSN")
        {

        }

    }
    public static class CommandRequestGSNExtension
    {
        /// <summary>
        /// Serial Number 
        /// </summary>
        /// <param name="gsmClient"></param>
        /// <returns></returns>
        public static CommandRequestGSN GSN(this GsmClient gsmClient) => new CommandRequestGSN(gsmClient);
    }
}
