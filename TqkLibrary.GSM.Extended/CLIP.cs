using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.GSM.Extended
{
    /// <summary>
    /// 
    /// </summary>
    public class CommandRequestCLIP : CommandRequest
    {
        internal CommandRequestCLIP(IGsmClient gsmClient) : base(gsmClient, "CLIP")
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public new async Task<ClipInfo?> ReadAsync(CancellationToken cancellationToken = default)
        {
            var result = await base.ReadAsync(cancellationToken);
            var clip = result.GetCommandResponse(Command);
            if (result.IsSuccess &&
                clip is not null &&
                clip.Arguments.Count() == 2 &&
                int.TryParse(clip.Arguments.First(), out int n) &&
                int.TryParse(clip.Arguments.Last(), out int m))
            {
                return new ClipInfo()
                {
                    CliPresentation = (CliPresentation)n,
                    Status = (ClipNetworkStatus)m,
                };
            }
            return null;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="cliPresentation"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<GsmCommandResult> WriteAsync(CliPresentation cliPresentation, CancellationToken cancellationToken = default)
            => base.WriteAsync(cancellationToken, (int)cliPresentation);

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public enum CliPresentation
        {
            Disable = 0,
            Enable = 1
        }
        /// <summary>
        /// status of the CLIP service on the GSM network
        /// </summary>
        public enum ClipNetworkStatus
        {
            /// <summary>
            /// CLIP not provisioned
            /// </summary>
            CLIPNotProvisioned = 0,
            /// <summary>
            /// CLIP provisioned
            /// </summary>
            CLIPProvisioned = 1,
            /// <summary>
            /// unknown (e.g. no network is present ) 
            /// </summary>
            Unknown = 2
        }
        public struct ClipInfo
        {
            public CliPresentation CliPresentation { get; set; }
            public ClipNetworkStatus Status { get; set; }
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }

    /// <summary>
    /// </summary>
    public static class CommandRequestCLIPExtension
    {
        /// <summary>
        /// Calling Line Identification Presentation
        /// </summary>
        /// <param name="gsmClient"></param>
        /// <returns></returns>
        public static CommandRequestCLIP CLIP(this IGsmClient gsmClient) => new CommandRequestCLIP(gsmClient);
    }
}
