using System;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace TqkLibrary.GSM.Extensions.Advances
{
    /// <summary>
    /// 
    /// </summary>
    public class CallingHelper
    {
        readonly GsmClient gsmClient;
        readonly SimEventUtils simEventUtils;
        internal CallingHelper(GsmClient gsmClient, SimEventUtils simEventUtils, string incommingPhoneNumber = null)
        {
            this.gsmClient = gsmClient ?? throw new ArgumentNullException(nameof(gsmClient));
            this.simEventUtils = simEventUtils ?? throw new ArgumentNullException(nameof(simEventUtils));
            this.IncommingPhoneNumber = incommingPhoneNumber;
        }
        /// <summary>
        /// 
        /// </summary>
        public string IncommingPhoneNumber { get; }

        const string fileName = "RAM:voice.wav";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<FileDownloadHelper> AnswerAsync(CancellationToken cancellationToken = default)
        {
            await gsmClient.SendCommandAsync("ATA\r\n", cancellationToken).ConfigureAwait(false);
            try { await gsmClient.QFDEL().WriteAsync("RAM:*").ConfigureAwait(false); } catch { }
            await gsmClient.QAUDRD().WriteAsync(
                CommandRequestQAUDRD.RecordControl.Start,
                fileName,
                CommandRequestQAUDRD.RecordFormat.WAV_PCM16,
                cancellationToken).ConfigureAwait(false);

            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            using var register = cancellationToken.Register(() => tcs.TrySetCanceled());
            Action action = () => tcs.TrySetResult(true);
            try
            {
                simEventUtils.OnEndCall += action;
                await tcs.Task.ConfigureAwait(false);
            }
            finally
            {
                simEventUtils.OnEndCall -= action;
            }

            await gsmClient.QAUDRD().WriteAsync(CommandRequestQAUDRD.RecordControl.Stop, cancellationToken).ConfigureAwait(false);
            await Task.Delay(100, cancellationToken);
            return new FileDownloadHelper(gsmClient, fileName);
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
