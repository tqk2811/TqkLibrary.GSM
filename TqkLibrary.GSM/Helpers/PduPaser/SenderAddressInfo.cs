using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TqkLibrary.GSM.Helpers.PduPaser.Interfaces;

namespace TqkLibrary.GSM.Helpers.PduPaser
{
    /// <summary>
    /// 
    /// </summary>
    public class SenderAddressInfo : IAddressInfo
    {
        private SenderAddressInfo()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        public byte AddressLength { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public AddressesType AddressesType { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<byte> Address { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<byte> GetData()
        {
            yield return AddressLength;
            yield return (byte)AddressesType;
            foreach (var item in Address)
            {
                yield return item;
            }
        }

        byte SenderByteLength => (byte)((AddressLength + AddressLength % 2) / 2);//make length even
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rawPdu"></param>
        /// <returns></returns>
        public static SenderAddressInfo Parse(Stream rawPdu)
        {
            SenderAddressInfo addressInfo = new SenderAddressInfo();
            addressInfo.AddressLength = (byte)rawPdu.ReadByte();
            addressInfo.AddressesType = (AddressesType)rawPdu.ReadByte();
            addressInfo.Address = rawPdu.Read(addressInfo.SenderByteLength);
            return addressInfo;
        }
    }
}
