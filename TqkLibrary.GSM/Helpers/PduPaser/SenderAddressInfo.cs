using TqkLibrary.GSM.Helpers.PduPaser.Enums;
using TqkLibrary.GSM.Helpers.PduPaser.Interfaces;

namespace TqkLibrary.GSM.Helpers.PduPaser
{
    /// <summary>
    /// 
    /// </summary>
    public class SenderAddressInfo : IAddressInfo
    {
        byte _ext_ton_npi = 0;
        private SenderAddressInfo()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public byte AddressLength { get; private set; }

        /// <summary>
        /// Numbering plan identification<br>
        /// </br>bit 0 1 2 3
        /// </summary>
        public NumberingPlanIdentification NPI
        {
            get
            {
                return (NumberingPlanIdentification)(_ext_ton_npi & 0b00001111);
            }
        }

        /// <summary>
        /// Type of number<br>
        /// </br>bit 4 5 6
        /// </summary>
        public TypeOfNumber TON
        {
            get
            {
                return (TypeOfNumber)((_ext_ton_npi & 0b01110000) >> 4);
            }
        }

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
            yield return _ext_ton_npi;
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
            addressInfo._ext_ton_npi = (byte)rawPdu.ReadByte();
            addressInfo.Address = rawPdu.Read(addressInfo.SenderByteLength);
            return addressInfo;
        }
    }
}
