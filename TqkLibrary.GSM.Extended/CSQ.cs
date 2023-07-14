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
    public class CommandRequestCSQ : CommandRequest
    {
        internal CommandRequestCSQ(IGsmClient gsmClient) : base(gsmClient, "CSQ")
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public new async Task<CSQ_ExecuteResponse> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var result = await base.ExecuteAsync(cancellationToken).ConfigureAwait(false);
            var csq = result.GetCommandResponse(Command);
            if (result.IsSuccess && csq != null && csq.Arguments.Count() >= 2)
            {
                if (int.TryParse(csq.Arguments.First(), out int rssi) && int.TryParse(csq.Arguments.Skip(1).First(), out int ber))
                {
                    return new CSQ_ExecuteResponse()
                    {
                        SignalStrengthIndication = rssi,
                        BitErrorRate = ber,
                    };
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public class CSQ_ExecuteResponse
        {
            internal CSQ_ExecuteResponse()
            {

            }
            /// <summary>
            /// 
            /// </summary>
            public int SignalStrengthIndication { get; internal set; }
            /// <summary>
            /// 
            /// </summary>
            public int BitErrorRate { get; internal set; }

            /// <summary>
            /// 
            /// </summary>
            public ESignalLevel SignalLevel
            {
                get
                {
                    switch (SignalStrengthIndication)
                    {
                        case >= 0 and <= 9: return ESignalLevel.Marginal;
                        case >= 10 and <= 14: return ESignalLevel.Ok;
                        case >= 15 and <= 19: return ESignalLevel.Good;
                        case >= 20 and <= 31: return ESignalLevel.Excellent;

                        case 99:
                        default: return ESignalLevel.Disconnect;
                    }
                }
            }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
            public enum ESignalLevel
            {
                Disconnect = 0,
                Marginal = 1,
                Ok = 2,
                Good = 3,
                Excellent = 4
            }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public static class CommandRequestCSQExtension
    {
        /// <summary>
        /// Signal Quality
        /// </summary>
        /// <param name="gsmClient"></param>
        /// <returns></returns>
        public static CommandRequestCSQ CSQ(this IGsmClient gsmClient) => new CommandRequestCSQ(gsmClient);
    }
}
