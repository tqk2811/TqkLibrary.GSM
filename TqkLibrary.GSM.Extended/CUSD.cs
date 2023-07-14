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
    public class CommandRequestCUSD : CommandRequest
    {
        internal CommandRequestCUSD(IGsmClient gsmClient) : base(gsmClient, "CUSD")
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        /// <param name="str"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<CUSD_Response> WriteAsync(CUSD_N n, string str, CancellationToken cancellationToken = default)
        {
            var result = await base.WriteAsync(cancellationToken, (int)n, str.ToAtString()).ConfigureAwait(false);
            var cusd = result.GetCommandResponse(Command);
            if (result.IsSuccess && cusd != null)
            {
                if (int.TryParse(cusd.Arguments.FirstOrDefault(), out int code))
                {
                    CUSD_Response response = new CUSD_Response();
                    response.CUSD_M = (CUSD_M)code;
                    response.Str = cusd.Arguments.Skip(1).FirstOrDefault();
                    if (int.TryParse(cusd.Arguments.Skip(2).FirstOrDefault(), out int dcs)) response.DCS = dcs;
                    return response;
                }
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        public class CUSD_Response
        {
            /// <summary>
            /// 
            /// </summary>
            public CUSD_M CUSD_M { get; internal set; }
            /// <summary>
            /// 
            /// </summary>
            public string Str { get; internal set; }
            /// <summary>
            /// 
            /// </summary>
            public int? DCS { get; internal set; }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return Str;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public enum CUSD_N
        {
            /// <summary>
            /// 
            /// </summary>
            Disable = 0,
            /// <summary>
            /// 
            /// </summary>
            Enable = 1,
        }
        /// <summary>
        /// 
        /// </summary>
        public enum CUSD_M
        {
            /// <summary>
            /// no further user action required (network initiated USSD-Notify, or no further information needed after mobile initiated operation). 
            /// </summary>
            NoFurtherUserActionRequired = 0,
            /// <summary>
            /// further user action required (network initiated USSD-Request, or further information needed after mobile initiated operation) 
            /// </summary>
            FurtherUserActionRequired = 1,
            /// <summary>
            /// USSD terminated by the network 
            /// </summary>
            USSDTerminatedByTheNetwork = 2,
            /// <summary>
            /// other local client has responded
            /// </summary>
            OtherLocalClientHasResponded = 3,
            /// <summary>
            /// operation not supported 
            /// </summary>
            OperationNotSupported = 4,
            /// <summary>
            /// network time out
            /// </summary>
            NetworkTimeOut = 5,
        }

    }
    /// <summary>
    /// 
    /// </summary>
    public static class CommandRequestCUSDExtension
    {
        /// <summary>
        /// Unstructured Supplementary Service Data 
        /// </summary>
        /// <param name="gsmClient"></param>
        /// <returns></returns>
        public static CommandRequestCUSD CUSD(this IGsmClient gsmClient) => new CommandRequestCUSD(gsmClient);
    }
}
