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
    public class CommandRequestQAUDRD : CommandRequest
    {
        internal CommandRequestQAUDRD(GsmClient gsmClient) : base(gsmClient, "QAUDRD")
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public new async Task<RecordState?> ReadAsync(CancellationToken cancellationToken = default)
        {
            var result = await base.ExecuteAsync(cancellationToken).ConfigureAwait(false);
            var qaudrd = result.GetCommandResponse(Command);
            if (result.IsSuccess && qaudrd != null && qaudrd.Arguments.Count() >= 1)
            {
                if (int.TryParse(qaudrd.Arguments.First(), out int state))
                {
                    return (RecordState)state;
                }
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="control">for <see cref="RecordControl.Stop"/> only</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<GsmCommandResult> WriteAsync(RecordControl control = RecordControl.Stop, CancellationToken cancellationToken = default)
            => base.WriteAsync(cancellationToken, (int)control);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        /// <param name="fileName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<GsmCommandResult> WriteAsync(RecordControl control, string fileName, CancellationToken cancellationToken = default)
            => base.WriteAsync(cancellationToken, (int)control, fileName.ToAtString());

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        /// <param name="fileName"></param>
        /// <param name="recordFormat"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<GsmCommandResult> WriteAsync(RecordControl control, string fileName, RecordFormat recordFormat, CancellationToken cancellationToken = default)
            => base.WriteAsync(cancellationToken, (int)control, fileName.ToAtString(), (int)recordFormat);


#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public enum RecordControl
        {
            Stop = 0,
            Start = 1,
        }
        public enum RecordState
        {
            NotRecording = 0,
            Recording = 1
        }
        public enum RecordFormat
        {
            AMR = 3,
            WAV_PCM16 = 13,
            WAV_ALAW = 14,
            WAV_ULAW = 15,
            WAV_ADPCM = 16
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }

    /// <summary>
    /// 
    /// </summary>
    public static class CommandRequestQAUDRDExtension
    {
        /// <summary>
        /// Download the File from the Storage
        /// </summary>
        /// <param name="gsmClient"></param>
        /// <returns></returns>
        public static CommandRequestQAUDRD QAUDRD(this GsmClient gsmClient) => new CommandRequestQAUDRD(gsmClient);
    }
}
