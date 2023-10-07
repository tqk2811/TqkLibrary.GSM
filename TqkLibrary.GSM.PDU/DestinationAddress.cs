namespace TqkLibrary.GSM.PDU
{
    /// <summary>
    /// TP-DA
    /// </summary>
    public class DestinationAddress : IDestinationAddress
    {
        //The EXT bit is always 1 meaning "no extension".
        byte _ext_ton_npi = 0b10000000;

        /// <summary>
        /// 
        /// </summary>
        public byte MessageReference { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public byte AddressLength { get; set; }

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
            set
            {
                _ext_ton_npi = (byte)((_ext_ton_npi & 0b111110000) | (byte)value);
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
            set
            {
                _ext_ton_npi = (byte)((_ext_ton_npi & 0b10001111) | ((byte)value << 4));
            }
        }

        /// <summary>
        /// Is false mean "no extension".
        /// </summary>
        public bool IsHaveExt
        {
            get
            {
                return (_ext_ton_npi & 0b10000000) == 0;//The EXT bit is 1 meaning "no extension"
            }
            set
            {
                _ext_ton_npi = (byte)((_ext_ton_npi & 0b01111111) | ((value ? 0 : 1) << 7));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<byte> Address { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IEnumerable<byte> GetData()
        {
            yield return MessageReference;
            yield return AddressLength;
            yield return _ext_ton_npi;
            foreach (var item in Address)
            {
                yield return item;
            }
        }
    }
}
