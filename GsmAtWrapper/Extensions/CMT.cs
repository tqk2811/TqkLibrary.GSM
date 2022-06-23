using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GsmAtWrapper.Extensions
{
    public static partial class GsmExtensions
    {
        public static async Task<CMTMessage> RegisterMessage(this GsmClient gsmClient, CancellationToken cancellationToken)
        {
            if (await gsmClient.WriteNewMessageIndicationsToTerminalEquipment(CNMI_Mode.Class2, CNMI_MT.SmsDeliver, cancellationToken).ConfigureAwait(false) &&
                await gsmClient.WritePreferredMessageStorage(CPMS_MEMR.SM, cancellationToken).ConfigureAwait(false))
            {
                return new CMTMessage(gsmClient);
            }
            return null;
        }
    }

    public class CMTMessage : IDisposable
    {
        readonly GsmClient gsmClient;
        public CMTMessage(GsmClient gsmClient)
        {
            this.gsmClient = gsmClient;
            gsmClient.OnCommandResponse += GsmClient_OnCommandResponse;
        }
        ~CMTMessage()
        {
            gsmClient.OnCommandResponse -= GsmClient_OnCommandResponse;
        }

        private void GsmClient_OnCommandResponse(string arg1, string[] arg2)
        {
            if ("CMT".Equals(arg1))
                ThreadPool.QueueUserWorkItem((o) => _GsmClient_OnCommandResponse(arg2));
        }

        private async void _GsmClient_OnCommandResponse(string[] arg2)
        {
            switch (await gsmClient.ReadMessageFormat())
            {
                case MessageFormat.PduMode:
                    //[<alpha>],<length><CR><LF><pdu>
                    break;

                case MessageFormat.TextMode:
                    // +CMT:<oa>,<alpha>,<scts>[,<tooa>,<fo>,<pid>,<dcs>,<sca>,<tosca>,<length>]<CR><LF><data>

                    break;

                default:
                    break;
            }
        }

        public void Dispose()
        {
            gsmClient.OnCommandResponse -= GsmClient_OnCommandResponse;
            GC.SuppressFinalize(this);
        }
    }

    public class Sms
    {
        public string OriginatorAddress { get; set; }
    }
}
