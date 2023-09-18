using TqkLibrary.GSM.PDU.Interfaces;

namespace TqkLibrary.GSM.PDU.UserDataHeaderIndicatorDatas
{
    /// <summary>
    /// 
    /// </summary>
    public class ConcatenatedSms16 : IConcatenatedSms, IUserData
    {
        readonly byte[] _data;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public ConcatenatedSms16(byte[] data)
        {
            if (data is null) throw new ArgumentNullException(nameof(data));
            if (data.Length != 4)
                throw new InvalidDataException($"{nameof(ConcatenatedSms16)} data length must be 4");
            _data = data;
        }
        /// <summary>
        /// 
        /// </summary>
        public ConcatenatedSms16() : this(new byte[4])
        {

        }
        /// <summary>
        /// 
        /// </summary>
        public ushort CSMSReferenceNumber
        {
            get => BitConverter.ToUInt16(_data, 0);
            set
            {
                _data[0] = (byte)value;
                _data[1] = (byte)(value >> 8);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public byte TotalNumberOfParts
        {
            get => _data[2];
            set => _data[2] = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public byte PartNumberInTheSequence
        {
            get => _data[3];
            set => _data[3] = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<byte> GetData()
        {
            return _data;
        }
    }
}
