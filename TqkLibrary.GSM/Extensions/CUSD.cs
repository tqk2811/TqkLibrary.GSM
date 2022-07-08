using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.GSM.Extensions
{
    public class CommandRequestCUSD : CommandRequest
    {
        internal CommandRequestCUSD(GsmClient gsmClient) : base(gsmClient, "CUSD")
        {

        }
        public async Task<CUSD_Response> Write(CUSD_N n, string str, CancellationToken cancellationToken = default)
        {
            var result = await base.Write(cancellationToken, (int)n, str.ToAtString()).ConfigureAwait(false);
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

    }
    public static class CommandRequestCUSDExtension
    {
        /// <summary>
        /// Unstructured Supplementary Service Data 
        /// </summary>
        /// <param name="gsmClient"></param>
        /// <returns></returns>
        public static CommandRequestCUSD CUSD(this GsmClient gsmClient) => new CommandRequestCUSD(gsmClient);
    }
    public class CUSD_Response
    {
        public CUSD_M CUSD_M { get; internal set; }
        public string Str { get; internal set; }
        public int? DCS { get; internal set; }

        public override string ToString()
        {
            return Str;
        }
    }
    public enum CUSD_N
    {
        Disable = 0,
        Enable = 1,
    }
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
