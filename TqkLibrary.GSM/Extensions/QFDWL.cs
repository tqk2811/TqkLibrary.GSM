using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.GSM.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public class CommandRequestQFDWL : CommandRequest
    {
        internal CommandRequestQFDWL(GsmClient gsmClient) : base(gsmClient, "QFDWL")
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public new async Task<FileData> WriteAsync(string fileName, CancellationToken cancellationToken = default)
        {
            var result = await base.WriteAsync(cancellationToken, fileName.ToAtString());
            var qfdwl = result.GetCommandResponse(Command);
            if (result.IsSuccess && qfdwl is not null && qfdwl.Arguments.Count() == 2 && int.TryParse(qfdwl.Arguments.First(), out int binarySize))
            {
                return new FileData(qfdwl.BinaryData, binarySize, qfdwl.Arguments.Last());
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public class FileData
        {
            internal FileData(IEnumerable<byte> bytes, int binarySize, string checksum)
            {
                if (string.IsNullOrWhiteSpace(checksum)) throw new ArgumentNullException(nameof(checksum));
                this.BinaryData = bytes;
                this.BinarySize = binarySize;
                this.CheckSum = checksum;
            }

            /// <summary>
            /// 
            /// </summary>
            public IEnumerable<byte> BinaryData { get; }

            /// <summary>
            /// 
            /// </summary>
            public int BinarySize { get; }
            /// <summary>
            /// 
            /// </summary>
            public string CheckSum { get; }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            /// <exception cref="DataCorruptedException"></exception>
            public byte[] GetAndCheck()
            {
                byte[] buffer = BinaryData is byte[]? (byte[])BinaryData : BinaryData.ToArray();
                if (buffer.Length != BinarySize)
                    throw new DataCorruptedException($"Data is wrong size BinaryData.Length = {buffer.Length}, BinarySize: {BinarySize}");
                if (!string.IsNullOrWhiteSpace(CheckSum))
                {
                    byte[] checksum = CheckSum.HexStringToByteArray();
                    byte[] calcCheckSum = buffer.CheckSum();

                    if (!calcCheckSum.SequenceEqual(checksum))
                        throw new DataCorruptedException($"Data checksum failed CheckSum: {CheckSum}, CalcCheckSum: {BitConverter.ToString(calcCheckSum).Replace("-", string.Empty)}");
                }
                return buffer;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public class DataCorruptedException : Exception
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="message"></param>
            public DataCorruptedException(string message) : base(message)
            {

            }
        }
    }

    /// <summary>
    /// </summary>
    public static class CommandRequestQFDWLExtension
    {
        /// <summary>
        /// Download the File from the Storage
        /// </summary>
        /// <param name="gsmClient"></param>
        /// <returns></returns>
        public static CommandRequestQFDWL QFDWL(this GsmClient gsmClient) => new CommandRequestQFDWL(gsmClient);
    }
}
