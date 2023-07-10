using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TqkLibrary.GSM.Helpers.PduPaser.Encrypts
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDecoder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="raw"></param>
        /// <param name="dataLength"></param>
        /// <param name="padding"></param>
        /// <returns></returns>
        string Decode(byte[] raw, int dataLength, int padding = 0);
    }
}
