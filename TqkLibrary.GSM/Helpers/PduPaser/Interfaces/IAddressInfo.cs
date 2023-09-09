using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TqkLibrary.GSM.Helpers.PduPaser.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAddressInfo
    {
        /// <summary>
        /// 
        /// </summary>
        byte AddressLength { get; }
        /// <summary>
        /// 
        /// </summary>
        AddressesType AddressesType { get; }
        /// <summary>
        /// 
        /// </summary>
        IEnumerable<byte> Address { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<byte> GetData();
    }
}
