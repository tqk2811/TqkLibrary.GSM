using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TqkLibrary.GSM.Extensions
{
    public enum MessageFormat
    {
        PduMode = 0,
        TextMode = 1
    }
    public static partial class GsmExtensions
    {
        /// <summary>
        /// 3.5.3.1.3 +CMGF - Message Format
        /// </summary>
        /// <param name="gsmClient"></param>
        /// <param name="messageFormat"></param>
        /// <returns></returns>
        public static Task<bool> WriteMessageFormat(this GsmClient gsmClient, MessageFormat messageFormat = MessageFormat.TextMode)
            => gsmClient.Write("CMGF", ((int)messageFormat).ToString())
            .GetTaskResult(x => x.IsSuccess);

        public static async Task<MessageFormat?> ReadMessageFormat(this GsmClient gsmClient)
        {
            var result = await gsmClient.Read("CMGF").ConfigureAwait(false);
            if (result.CommandResponses.ContainsKey("CMGF") &&
                result.CommandResponses["CMGF"].Arguments.Count() > 0 &&
                int.TryParse(result.CommandResponses["CMGF"].Arguments.First(), out int val))
            {
                if (val == (int)MessageFormat.PduMode) return MessageFormat.PduMode;
                if (val == (int)MessageFormat.TextMode) return MessageFormat.TextMode;
            }
            return null;
        }

        public static Task<bool> TestMessageFormat(this GsmClient gsmClient)
            => gsmClient.Read("CMGF")
            .GetTaskResult(x => x.IsSuccess);
    }
}
