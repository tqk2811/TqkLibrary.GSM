using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.GSM.Extensions
{
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
    public static partial class GsmExtensions
    {
        /// <summary>
        /// 3.5.2.3.13 +CUSD - Unstructured Supplementary Service Data 
        /// </summary>
        /// <returns></returns>
        public static async Task<CUSD_Response> WriteUnstructuredSupplementaryServiceData(
            this GsmClient gsmClient,
            CUSD_N n,
            string str,
            CancellationToken cancellationToken = default)
        {
            var result = await gsmClient.Write("CUSD", cancellationToken, (int)n, str.ToAtString()).ConfigureAwait(false);
            if(result.IsSuccess && result.CommandResponses.ContainsKey("CUSD"))
            {
                if (int.TryParse(result.CommandResponses["CUSD"].Arguments.FirstOrDefault(),out int code))
                {
                    CUSD_Response response = new CUSD_Response();
                    response.CUSD_M = (CUSD_M)code;
                    response.Str = result.CommandResponses["CUSD"].Arguments.Skip(1).FirstOrDefault();
                    if (int.TryParse(result.CommandResponses["CUSD"].Arguments.Skip(2).FirstOrDefault(), out int dcs)) response.DCS = dcs;
                    return response;
                }
            }
            return null;
        }
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
}
