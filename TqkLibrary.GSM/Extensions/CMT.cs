﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TqkLibrary.GSM.Helpers.PduPaser;
using TqkLibrary.GSM.Helpers.PduPaser.Decoders;
using Nito.AsyncEx;
namespace TqkLibrary.GSM.Extensions
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
        public event Action<ISms> OnSmsReceived;
        readonly Dictionary<byte, List<Message>> pdu_cache = new Dictionary<byte, List<Message>>();
        readonly AsyncLock asyncLock = new AsyncLock();
        public CMTMessage(GsmClient gsmClient)
        {
            this.gsmClient = gsmClient;
            gsmClient.OnCommandResponse += GsmClient_OnCommandResponse;
        }



        ~CMTMessage()
        {
            gsmClient.OnCommandResponse -= GsmClient_OnCommandResponse;
        }

        private void GsmClient_OnCommandResponse(GsmCommandResponse commandData)
        {
            if ("CMT".Equals(commandData?.Command))
                ThreadPool.QueueUserWorkItem((o) => _GsmClient_OnCommandResponse(commandData));
        }

        private async void _GsmClient_OnCommandResponse(GsmCommandResponse commandData)
        {
            switch (await gsmClient.ReadMessageFormat())
            {
                case MessageFormat.PduMode:
                    //[<alpha>],<length><CR><LF><pdu>
                    PDU pdu = PDU.TryParse(commandData.Data.HexStringToByteArray());
                    if (pdu != null && (pdu.PduHeader & PDU.SMS_DELIVER) == PDU.SMS_DELIVER)
                    {
                        Message message = new Message(pdu);
                        if (message.IsSplit)
                        {
                            using (await asyncLock.LockAsync().ConfigureAwait(false))
                            {
                                if (pdu_cache.ContainsKey(pdu.UDH.CSMSReferenceNumber))
                                {
                                    pdu_cache[pdu.UDH.CSMSReferenceNumber].Add(message);
                                    if (pdu_cache[pdu.UDH.CSMSReferenceNumber].Count == pdu.UDH.TotalNumberOfParts)
                                    {
                                        SmsPdu smsPdu = new SmsPdu(pdu_cache[pdu.UDH.CSMSReferenceNumber]);
                                        pdu_cache.Remove(pdu.UDH.CSMSReferenceNumber);
                                        OnSmsReceived?.Invoke(smsPdu);
                                    }
                                }
                                else
                                {
                                    pdu_cache[pdu.UDH.CSMSReferenceNumber] = new List<Message>() { message };
                                }
                            }
                        }
                        else
                        {
                            SmsPdu smsPdu = new SmsPdu(message);
                            OnSmsReceived?.Invoke(smsPdu);
                        }
                    }
                    break;

                case MessageFormat.TextMode:
                    // +CMT:<oa>,<alpha>,<scts>[,<tooa>,<fo>,<pid>,<dcs>,<sca>,<tosca>,<length>]<CR><LF><data>
                    if ((commandData.Arguments.Count() == 3 || commandData.Arguments.Count() == 10) && !string.IsNullOrWhiteSpace(commandData.Data))
                    {
                        Sms sms = new Sms();
                        sms.From = commandData.Arguments.FirstOrDefault()?.Trim('"');
                        //sms.AlphanumericRepresentation = commandData.Arguments.Skip(1).FirstOrDefault()?.Trim('"');
                        if (DateTime.TryParse(commandData.Arguments.Skip(2).FirstOrDefault(), out DateTime r)) sms.ArrivalTime = r;
                        sms.Message = commandData.Data;
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

    public interface ISms
    {
        string From { get; }
        string Message { get; }
        DateTime? ArrivalTime { get; }
    }

    internal class Sms : ISms
    {
        public string From { get; set; }

        public string Message { get; set; }

        public DateTime? ArrivalTime { get; set; }
    }

    internal class SmsPdu : ISms
    {
        readonly List<Message> messages = new List<Message>();
        public SmsPdu(Message message)
        {
            messages.Add(message ?? throw new ArgumentNullException(nameof(message)));
        }
        public SmsPdu(IEnumerable<Message> messages)
        {
            if (messages.Any(x => x == null || !x.IsSplit || x.SplitCount != messages.Count()) ||
                messages.GroupBy(x => x.SplitId).Count() != 1 ||
                messages.GroupBy(x => x.SplitIndex).Count() != messages.Count())
                throw new InvalidOperationException($"messages is wrong");

            this.messages.AddRange(messages.OrderBy(x => x.SplitIndex));
        }

        public string From => messages.First().SenderNumber;
        public string Message => string.Join("", messages.Select(x => x.Content));
        public DateTime? ArrivalTime => messages.First().DateTime;
    }
}