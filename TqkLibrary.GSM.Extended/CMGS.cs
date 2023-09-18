using TqkLibrary.GSM.PDU;
using static TqkLibrary.GSM.Extended.CommandRequestCMGF;

namespace TqkLibrary.GSM.Extended
{
    /// <summary>
    /// 
    /// </summary>
    public class CommandRequestCMGS : CommandRequest
    {
        internal CommandRequestCMGS(IGsmClient gsmClient) : base(gsmClient, "CMGS")
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public TimeSpan ValidityPeriod { get; set; } = TimeSpan.Zero;

        /// <summary>
        /// 
        /// </summary>
        public MessageFormat? ForceFormat { get; set; } = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="destPhoneNumber"></param>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public async Task<GsmCommandResult> WriteAsync(string destPhoneNumber, string message, CancellationToken cancellationToken = default)
        {
            byte[] dataWrite = null;
            Action<PromptEvent> action = async (PromptEvent promptEvent) =>
            {
                using Stream stream = promptEvent.GetStream();
                try
                {
                    await stream.WriteAsync(dataWrite, 0, dataWrite.Length);
                }
                finally
                {
                    promptEvent.SendCtrlZ();
                }
            };
            MessageFormat? currentMessageFormat = await GsmClient.CMGF().ReadAsync();
            if (currentMessageFormat.HasValue && ForceFormat.HasValue)
            {
                await GsmClient.CMGF().WriteAsync(ForceFormat.Value);
            }
            try
            {
                MessageFormat? _currentMessageFormat = await GsmClient.CMGF().ReadAsync();
                switch (_currentMessageFormat)
                {
                    case MessageFormat.PduMode:
                        {
                            List<Pdu> pdus = Pdu.Create(destPhoneNumber, message, this.ValidityPeriod).ToList();
                            GsmCommandResult gsmCommandResult = null;
                            foreach (var pdu in pdus)
                            {
                                dataWrite = (byte[])pdu;
                                try
                                {
                                    GsmClient.OnPromptEvent += action;

                                    gsmCommandResult = await base.WriteAsync(cancellationToken, dataWrite.Length);
                                }
                                finally
                                {
                                    GsmClient.OnPromptEvent -= action;
                                }
                            }
                            return gsmCommandResult;
                        }

                    case MessageFormat.TextMode:
                        {
                            try
                            {
                                if (message.Any(x => !(x == 13 || (x >= 32 && x < 127))))
                                    throw new InvalidDataException($"message has invalid character");

                                dataWrite = Encoding.ASCII.GetBytes(message);
                                if (dataWrite.Length > 160)
                                    throw new InvalidDataException($"message is large than 160 bytes");

                                GsmClient.OnPromptEvent += action;

                                return await base.WriteAsync(cancellationToken, destPhoneNumber.ToAtString());
                            }
                            finally
                            {
                                GsmClient.OnPromptEvent -= action;
                            }
                        }

                    default:
                        throw new NotSupportedException(_currentMessageFormat?.ToString());
                }
            }
            finally
            {
                if (currentMessageFormat.HasValue && ForceFormat.HasValue && currentMessageFormat.Value != ForceFormat.Value)
                {
                    await GsmClient.CMGF().WriteAsync(currentMessageFormat.Value);
                }
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public static class CommandRequestCMGSExtension
    {
        /// <summary>
        /// Send Message
        /// </summary>
        /// <param name="gsmClient"></param>
        /// <returns></returns>
        public static CommandRequestCMGS CMGS(this IGsmClient gsmClient) => new CommandRequestCMGS(gsmClient);
    }
}
