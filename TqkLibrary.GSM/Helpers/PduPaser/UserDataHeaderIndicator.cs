using System.Collections;
using System.Linq;
using TqkLibrary.GSM.Helpers.PduPaser.Enums;
using TqkLibrary.GSM.Helpers.PduPaser.Interfaces;
using TqkLibrary.GSM.Helpers.PduPaser.UserDataHeaderIndicatorDatas;

namespace TqkLibrary.GSM.Helpers.PduPaser
{
    /// <summary>
    /// TP-UDHI
    /// </summary>
    public class UserDataHeaderIndicator
    {
        /// <summary>
        /// 
        /// </summary>
        public UserDataHeaderIndicator()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public byte UDHLength => (byte)(2 + IELength);

        /// <summary>
        /// Information Element Identifier
        /// </summary>
        public InformationElementIdentifier InformationElementIdentifier { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public byte IELength => (byte)(UserData?.GetData()?.Count() ?? 0);

        /// <summary>
        /// TP-UD
        /// </summary>
        public IUserData UserData { get; set; }


        //https://en.wikipedia.org/wiki/User_Data_Header
        //https://en.wikipedia.org/wiki/Concatenated_SMS
        /// <summary>
        /// 
        /// </summary>
        public int Padding => (7 - ((8 * (1 + UDHLength)) % 7)) % 7;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="userDataHeader"></param>
        public static explicit operator byte[](UserDataHeaderIndicator userDataHeader) => userDataHeader.GetData().ToArray();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<byte> GetData()
        {
            yield return UDHLength;
            yield return (byte)InformationElementIdentifier;
            yield return IELength;
            foreach (var item in UserData.GetData())
            {
                yield return item;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidDataException"></exception>
        public static UserDataHeaderIndicator Read(Stream stream)
        {
            byte length = (byte)stream.ReadByte();
            byte[] buffer = new byte[length + 1];
            buffer[0] = length;
            stream.Read(buffer, 1, length);
            return Parse(buffer);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidDataException"></exception>
        public static UserDataHeaderIndicator Parse(byte[] buffer)
        {
            if (buffer is null)
                throw new ArgumentNullException(nameof(buffer));
            if (buffer.Length <= 3)
                throw new InvalidDataException($"{nameof(buffer)} length must be large than 3");

            byte UDHLength = buffer.First();
            if (UDHLength != buffer.Length - 1)
                throw new InvalidDataException($"{nameof(UDHLength)} value is invalid  (UDHLength != buffer.Length - 1)");

            UserDataHeaderIndicator udh = new UserDataHeaderIndicator();
            udh.InformationElementIdentifier = (InformationElementIdentifier)buffer.Skip(1).First();

            byte IELength = buffer.Skip(2).First();
            var data = buffer.Skip(3).Take(IELength).ToArray();
            if (data.Length != IELength)
                throw new InvalidDataException($"udh.Data is invalid Length");


            switch (udh.InformationElementIdentifier)
            {
                case InformationElementIdentifier.ConcatenatedShortMessages:
                    udh.UserData = new ConcatenatedSms8(data);
                    break;

                case InformationElementIdentifier.ConcatenatedShortMessage16BitReferenceNumber:
                    udh.UserData = new ConcatenatedSms16(data);
                    break;

                default:
                    udh.UserData = new DefaultUserDataHeaderData(data);
                    break;
            }
            return udh;
        }
    }
}
