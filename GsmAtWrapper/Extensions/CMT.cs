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
        public static async Task<CMTMessage> RegisterMessage(this GsmClient gsmClient, CancellationToken cancellationToken = default)
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
        public event Action<Sms> OnSmsReceived;

        ~CMTMessage()
        {
            gsmClient.OnCommandResponse -= GsmClient_OnCommandResponse;
        }

        private void GsmClient_OnCommandResponse(string cmd, string[] args, string data)
        {
            if ("CMT".Equals(cmd))
                ThreadPool.QueueUserWorkItem((o) => _GsmClient_OnCommandResponse(args, data));
        }

        private async void _GsmClient_OnCommandResponse(string[] args, string data)
        {
            switch (await gsmClient.ReadMessageFormat())
            {
                case MessageFormat.PduMode:
                    //[<alpha>],<length><CR><LF><pdu>
                    break;

                case MessageFormat.TextMode:
                    // +CMT:<oa>,<alpha>,<scts>[,<tooa>,<fo>,<pid>,<dcs>,<sca>,<tosca>,<length>]<CR><LF><data>
                    if ((args.Length == 3 || args.Length == 10) && !string.IsNullOrWhiteSpace(data))
                    {
                        Sms sms = new Sms();
                        sms.OriginatorAddress = args.FirstOrDefault()?.Trim('"');
                        sms.AlphanumericRepresentation = args.Skip(1).FirstOrDefault()?.Trim('"');
                        if (DateTime.TryParse(args.Skip(2).FirstOrDefault(), out DateTime r)) sms.ArrivalTime = r;
                        sms.Data = data;
                        OnSmsReceived?.Invoke(sms);
                    }
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
        public string OriginatorAddress { get; internal set; }
        public string AlphanumericRepresentation { get; internal set; }
        public DateTime? ArrivalTime { get; internal set; }
        public string Data { get; internal set; }
    }
}
