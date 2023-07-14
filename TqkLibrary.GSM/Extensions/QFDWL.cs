using System;
using System.Collections.Generic;
using System.IO;
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
            var qflst = await GsmClient.QFLST().WriteAsync(fileName, cancellationToken);
            if (qflst.Count == 0)
                return null;

            return await WriteAsync(qflst.First(), cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<FileData> WriteAsync(CommandRequestQFLST.FileInfo fileInfo, CancellationToken cancellationToken = default)
        {
            byte[] buffer = new byte[fileInfo.FileSize];
            int offset = 0;
            Action<ConnectDataEvent> action = async (ConnectDataEvent connectDataEvent) =>
            {
                using Stream stream = connectDataEvent.GetStream();
                while (offset < buffer.Length)
                {
                    int byteRead = await stream.ReadAsync(buffer, offset, buffer.Length - offset);
#if DEBUG
                    if (offset == 0)
                    {
                        Console.WriteLine(Encoding.UTF8.GetString(buffer, 0, byteRead));
                    }
                    Console.WriteLine($"--------\t\tDownload Connect: {offset}/{fileInfo.FileSize}");
#endif
                    offset += byteRead;
                }
            };
            try
            {
                GsmClient.OnConnectDataEvent += action;
                var result = await base.WriteAsync(cancellationToken, fileInfo.Path.ToAtString());
                var qfdwl = result.GetCommandResponse(Command);
                if (result.IsSuccess && qfdwl is not null && qfdwl.Arguments.Count() == 2 && int.TryParse(qfdwl.Arguments.First(), out int binarySize))
                {
                    return new FileData(buffer, binarySize, qfdwl.Arguments.Last());
                }
                return null;
            }
            finally
            {
                GsmClient.OnConnectDataEvent -= action;
            }
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
