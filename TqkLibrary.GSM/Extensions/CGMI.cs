using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.GSM.Extensions
{
    public class CommandRequestCGMI : CommandRequest, IExecuteFirstData
    {
        internal CommandRequestCGMI(GsmClient gsmClient) : base(gsmClient, "CGMI")
        {

        }
    }

    public static class CommandRequestCGMIExtension
    {
        /// <summary>
        /// Request Manufacturer Identification
        /// </summary>
        /// <param name="gsmClient"></param>
        /// <returns></returns>
        public static CommandRequestCGMI CGMI(this GsmClient gsmClient) => new CommandRequestCGMI(gsmClient);
    }
}
