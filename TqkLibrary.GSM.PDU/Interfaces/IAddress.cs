using System.Collections.Generic;
using TqkLibrary.GSM.PDU.Enums;

namespace TqkLibrary.GSM.PDU.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAddress
    {
        /// <summary>
        /// 
        /// </summary>
        byte AddressLength { get; }
        /// <summary>
        /// Type of number
        /// </summary>
        TypeOfNumber TON { get; }
        /// <summary>
        /// Numbering plan identification
        /// </summary>
        NumberingPlanIdentification NPI { get; }
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
