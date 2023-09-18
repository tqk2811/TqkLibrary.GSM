using Nito.AsyncEx;
using TqkLibrary.GSM.Helpers.PduPaser.Interfaces;
using static TqkLibrary.GSM.Extended.CommandRequestCMGF;
using static TqkLibrary.GSM.Extended.CommandRequestCNMI;
using static TqkLibrary.GSM.Extended.CommandRequestCPMS;

namespace TqkLibrary.GSM.Extended.Advances
{
    /// <summary>
    /// 
    /// </summary>
    public static class CMTMessageExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gsmClient"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<CMTMessage> RegisterMessageAsync(this IGsmClient gsmClient, CancellationToken cancellationToken = default)
        {
            if (await gsmClient.CNMI().WriteAsync(CNMI_Mode.Class2, CNMI_MT.SmsDeliver, cancellationToken).ConfigureAwait(false) &&
                await gsmClient.CPMS().WriteAsync(CPMS_MEMR.SM, cancellationToken).ConfigureAwait(false))
            {
                return new CMTMessage(gsmClient);
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gsmClient"></param>
        /// <returns></returns>
        public static CMTMessage RegisterMessage(this IGsmClient gsmClient)
            => new CMTMessage(gsmClient);
    }

    /// <summary>
    /// 
    /// </summary>
    public class CMTMessage : IDisposable
    {
        readonly IGsmClient gsmClient;
        /// <summary>
        /// 
        /// </summary>
        public event Action<ISms> OnSmsReceived;
        readonly Dictionary<UInt16, List<Message>> pdu_cache = new Dictionary<UInt16, List<Message>>();
        readonly AsyncLock asyncLock = new AsyncLock();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gsmClient"></param>
        public CMTMessage(IGsmClient gsmClient)
        {
            this.gsmClient = gsmClient;
            gsmClient.OnCommandResponse += GsmClient_OnCommandResponse;
        }
        /// <summary>
        /// 
        /// </summary>
        ~CMTMessage()
        {
            Dispose(false);
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing)
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
            try
            {
                switch (await gsmClient.CMGF().ReadAsync())
                {
                    case MessageFormat.PduMode:
                        //[<alpha>],<length><CR><LF><pdu>
                        PDU pdu = PDU.TryParse(commandData.Data.HexStringToByteArray());
                        if (pdu?.PduHeader?.Type == PduType.SmsDeliver)
                        {
                            Message message = new Message(pdu);
                            if (message.IsSplit)
                            {
                                using (await asyncLock.LockAsync().ConfigureAwait(false))
                                {
                                    IConcatenatedSms concatenatedSms = pdu.UserDataHeaderIndicator.UserData as IConcatenatedSms;
                                    if (pdu_cache.ContainsKey(concatenatedSms.CSMSReferenceNumber))
                                    {
                                        pdu_cache[concatenatedSms.CSMSReferenceNumber].Add(message);
                                        if (pdu_cache[concatenatedSms.CSMSReferenceNumber].Count == concatenatedSms.TotalNumberOfParts)
                                        {
                                            SmsPdu smsPdu = new SmsPdu(pdu_cache[concatenatedSms.CSMSReferenceNumber]);
                                            pdu_cache.Remove(concatenatedSms.CSMSReferenceNumber);
                                            OnSmsReceived?.Invoke(smsPdu);
                                        }
                                    }
                                    else
                                    {
                                        pdu_cache[concatenatedSms.CSMSReferenceNumber] = new List<Message>() { message };
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
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine($"{ex.GetType().FullName}: {ex.Message}, {ex.StackTrace}");
#endif
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public interface ISms
        {
            /// <summary>
            /// 
            /// </summary>
            string From { get; }
            /// <summary>
            /// 
            /// </summary>
            string Message { get; }
            /// <summary>
            /// 
            /// </summary>
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
}
