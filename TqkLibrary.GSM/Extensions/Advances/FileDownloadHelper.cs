﻿using System;
using System.Threading.Tasks;
using System.Threading;

namespace TqkLibrary.GSM.Extensions.Advances
{
    /// <summary>
    /// 
    /// </summary>
    public class FileDownloadHelper
    {
        readonly GsmClient gsmClient;
        readonly string filePath;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gsmClient"></param>
        /// <param name="filePath"></param>
        public FileDownloadHelper(GsmClient gsmClient, string filePath)
        {
            this.gsmClient = gsmClient ?? throw new ArgumentNullException(nameof(gsmClient));
            if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentNullException(nameof(filePath));
            this.filePath = filePath;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="CommandRequestQFDWL.DataCorruptedException"></exception>
        public Task<CommandRequestQFDWL.FileData> DownloadAsync(CancellationToken cancellationToken = default)
        {
            return gsmClient.QFDWL().WriteAsync(filePath, cancellationToken);
        }
    }
}
