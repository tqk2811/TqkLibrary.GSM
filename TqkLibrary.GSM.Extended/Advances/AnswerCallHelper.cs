﻿namespace TqkLibrary.GSM.Extended.Advances
{
    /// <summary>
    /// 
    /// </summary>
    public class AnswerCallHelper
    {
        readonly IGsmClient gsmClient;
        readonly SimEventUtils simEventUtils;
        internal AnswerCallHelper(IGsmClient gsmClient, SimEventUtils simEventUtils, string incommingPhoneNumber = null)
        {
            this.gsmClient = gsmClient ?? throw new ArgumentNullException(nameof(gsmClient));
            this.simEventUtils = simEventUtils ?? throw new ArgumentNullException(nameof(simEventUtils));
            IncommingPhoneNumber = incommingPhoneNumber;
        }
        /// <summary>
        /// 
        /// </summary>
        public string IncommingPhoneNumber { get; }

        /// <summary>
        /// 
        /// </summary>
        public string FilePath { get; set; } = $"RAM:{Guid.NewGuid().ToString().Trim('{', '}')}.wav";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<GsmCommandResult> DeleteFileAsync(string filePath = "RAM:*", CancellationToken cancellationToken = default)
        {
            return gsmClient.QFDEL().WriteAsync(filePath, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listenTimeout">timeout in milisecond</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<FileDownloadHelper> AnswerAsync(int listenTimeout = 0, CancellationToken cancellationToken = default)
        {
            //stop old 
            try { await gsmClient.QAUDRD().WriteAsync(CommandRequestQAUDRD.RecordControl.Stop, cancellationToken).ConfigureAwait(false); } catch { }
            //try { await DeleteFileAsync(FilePath, cancellationToken).ConfigureAwait(false); } catch { }

            await gsmClient.SendCommandAsync("ATA\r\n", cancellationToken).ConfigureAwait(false);
            await gsmClient.QAUDRD().WriteAsync(
                CommandRequestQAUDRD.RecordControl.Start,
                FilePath,
                CommandRequestQAUDRD.RecordFormat.WAV_PCM16,
                cancellationToken).ConfigureAwait(false);

            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            using var register = cancellationToken.Register(() => tcs.TrySetCanceled());
            Action action = () => tcs.TrySetResult(true);

            using CancellationTokenSource listenTimeout_cts = listenTimeout > 0 ? new CancellationTokenSource(listenTimeout) : null;

            try
            {
                using var listenTimeout_cts_register = listenTimeout_cts?.Token.Register(
                    () =>
                    {
                        _ = gsmClient.CHUP().ExecuteAsync();
                        tcs.TrySetResult(true);
                    }
                    );
                simEventUtils.OnEndCall += action;
                await tcs.Task.ConfigureAwait(false);
            }
            finally
            {
                simEventUtils.OnEndCall -= action;
            }

            await gsmClient.QAUDRD().WriteAsync(CommandRequestQAUDRD.RecordControl.Stop, cancellationToken).ConfigureAwait(false);
            await Task.Delay(100, cancellationToken);
            return new FileDownloadHelper(gsmClient, FilePath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task HangupAsync(CancellationToken cancellationToken = default)
        {
            return gsmClient.CVHU().ExecuteAsync(cancellationToken);
        }
    }
}
