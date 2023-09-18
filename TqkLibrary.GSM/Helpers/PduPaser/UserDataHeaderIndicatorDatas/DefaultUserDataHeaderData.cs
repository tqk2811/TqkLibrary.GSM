using TqkLibrary.GSM.Helpers.PduPaser.Interfaces;

namespace TqkLibrary.GSM.Helpers.PduPaser.UserDataHeaderIndicatorDatas
{
    /// <summary>
    /// 
    /// </summary>
    public class DefaultUserDataHeaderData : IUserData
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public byte this[int index]
        {
            get => _data[index];
            set => _data[index] = value;
        }

        readonly byte[] _data;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public DefaultUserDataHeaderData(byte[] data)
        {
            if (data is null) throw new ArgumentNullException(nameof(data));
            this._data = data;
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
