using TqkLibrary.GSM.Helpers.PduPaser.Interfaces;

namespace TqkLibrary.GSM.Helpers.PduPaser.UserDataHeaderIndicatorDatas
{
    /// <summary>
    /// 
    /// </summary>
    public class ConcatenatedSms8 : IConcatenatedSms, IUserData
    {
        readonly byte[] _data;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public ConcatenatedSms8(byte[] data)
        {
            if (data is null) throw new ArgumentNullException(nameof(data));
            if (data.Length != 3)
                throw new InvalidDataException($"{nameof(ConcatenatedSms8)} data length must be 3");
            _data = data.ToArray();
        }
        /// <summary>
        /// 
        /// </summary>
        public ConcatenatedSms8() : this(new byte[3])
        {

        }
        /// <summary>
        /// 
        /// </summary>
        public byte CSMSReferenceNumber
        {
            get => _data[0];
            set => _data[0] = value;
        }
        /// <summary>
        /// 
        /// </summary>
        public byte TotalNumberOfParts
        {
            get => _data[1];
            set => _data[1] = value;
        }
        /// <summary>
        /// 
        /// </summary>
        public byte PartNumberInTheSequence
        {
            get => _data[2];
            set => _data[2] = value;
        }

        ushort IConcatenatedSms.CSMSReferenceNumber => CSMSReferenceNumber;

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
