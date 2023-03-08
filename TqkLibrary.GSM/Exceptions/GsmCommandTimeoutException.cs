using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TqkLibrary.GSM.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class GsmCommandTimeoutException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public GsmCommandTimeoutException(string message) : base($"Command Timeout \"{message}\"")
        {

        }
    }
}
