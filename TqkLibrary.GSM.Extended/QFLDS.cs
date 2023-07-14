namespace TqkLibrary.GSM.Extended
{
    /// <summary>
    /// 
    /// </summary>
    public class CommandRequestQFLDS : CommandRequest
    {
        internal CommandRequestQFLDS(IGsmClient gsmClient) : base(gsmClient, "QFLDS")
        {

        }

        /// <summary>
        /// </summary>
        /// <param name="namePattern">
        /// “UFS” UFS files in flash<br></br>
        /// “RAM” RAM files in the random storage<br></br>
        /// “SD” SD files in SD card
        /// </param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public new async Task<StorageSize> WriteAsync(string namePattern, CancellationToken cancellationToken = default)
        {
            var result = await base.WriteAsync(cancellationToken, namePattern.ToAtString()).ConfigureAwait(false);
            var qflds = result.GetCommandResponse(Command);
            if (result.IsSuccess && qflds != null && qflds.Arguments.Count() >= 2)
            {
                if (int.TryParse(qflds.Arguments.First(), out int freesize) && int.TryParse(qflds.Arguments.Skip(1).First(), out int totalsize))
                {
                    return new StorageSize(freesize, totalsize);
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>ufsfilesize and ufsfilenumber</returns>
        public new async Task<UfsInfo> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var result = await base.ExecuteAsync(cancellationToken).ConfigureAwait(false);
            var qflds = result.GetCommandResponse(Command);
            if (result.IsSuccess && qflds != null && qflds.Arguments.Count() >= 2)
            {
                if (int.TryParse(qflds.Arguments.First(), out int ufsfilesize) && int.TryParse(qflds.Arguments.Skip(1).First(), out int ufsfilenumber))
                {
                    return new UfsInfo(ufsfilesize, ufsfilenumber);
                }
            }
            return null;
        }


        /// <summary>
        /// 
        /// </summary>
        public class StorageSize
        {
            internal StorageSize(int freeSize, int totalSize)
            {
                FreeSize = freeSize;
                TotalSize = totalSize;
            }
            /// <summary>
            /// 
            /// </summary>
            public int FreeSize { get; }
            /// <summary>
            /// 
            /// </summary>
            public int TotalSize { get; }
        }
        /// <summary>
        /// 
        /// </summary>
        public class UfsInfo
        {
            internal UfsInfo(int ufsfilesize, int ufsfilenumber)
            {
                UfsFileSize = ufsfilesize;
                UfsFileNumber = ufsfilenumber;
            }
            /// <summary>
            /// 
            /// </summary>
            public int UfsFileSize { get; }
            /// <summary>
            /// 
            /// </summary>
            public int UfsFileNumber { get; }
        }
    }

    /// <summary>
    /// </summary>
    public static class CommandRequestQFLDSExtension
    {
        /// <summary>
        /// List Files
        /// </summary>
        /// <param name="gsmClient"></param>
        /// <returns></returns>
        public static CommandRequestQFLDS QFLDS(this IGsmClient gsmClient) => new CommandRequestQFLDS(gsmClient);
    }
}
