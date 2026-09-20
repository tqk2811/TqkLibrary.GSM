namespace TqkLibrary.GSM.Extended.Advances
{
    /// <summary>
    /// 
    /// </summary>
    public class SimEventUtils : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        public event Action OnSimPlugIn;
        /// <summary>
        /// 
        /// </summary>
        public event Action OnSimPlugOut;
        /// <summary>
        /// 
        /// </summary>
        public event Action OnProviderConnected;
        /// <summary>
        /// Raised when every registration indicator the modem reports has dropped.
        /// </summary>
        public event Action OnProviderDisconnected;
        /// <summary>
        /// Raised on the "RDY" unsolicited code, when the modem has finished booting and
        /// starts accepting AT commands.
        /// </summary>
        public event Action OnModuleReady;
        /// <summary>
        ///
        /// </summary>
        public event Action<AnswerCallHelper> OnCalling;
        /// <summary>
        /// 
        /// </summary>
        public event Action<AnswerCallHelper> OnCallingClip;
        /// <summary>
        /// 
        /// </summary>
        public event Action OnEndCall;



        readonly IGsmClient gsmClient;
        /// <summary>
        /// Registration state per indicator ("CREG", "CGREG", "CEREG", "Call Ready"). A modem can be
        /// attached on one indicator while reporting not-registered on another - a 3G-only SIM answers
        /// +CREG: 1,5 and +CEREG: 1,0 at the same time - so the provider counts as connected while any
        /// single indicator says it is, instead of the last one received winning.
        /// </summary>
        readonly Dictionary<string, bool> _registrationIndicators = new Dictionary<string, bool>();
        bool _isProviderConnected = false;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gsmClient"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public SimEventUtils(IGsmClient gsmClient)
        {
            this.gsmClient = gsmClient ?? throw new ArgumentNullException(nameof(gsmClient));
            gsmClient.OnUnknowReceived += GsmClient_OnUnknowReceived;
            gsmClient.OnCommandResponse += GsmClient_OnCommandResponse;
        }
        /// <summary>
        /// 
        /// </summary>
        ~SimEventUtils()
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
            gsmClient.OnUnknowReceived -= GsmClient_OnUnknowReceived;
            gsmClient.OnCommandResponse -= GsmClient_OnCommandResponse;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task EnableClip(CancellationToken cancellationToken = default)
        {
            await gsmClient.CLIP().WriteAsync(CommandRequestCLIP.CliPresentation.Enable, cancellationToken);
        }


        private void GsmClient_OnCommandResponse(GsmCommandResponse obj)
        {
            switch (obj?.Command)
            {
                case "CPIN":
                    {
                        var arg = obj.Arguments.FirstOrDefault()?.Trim('"');
                        switch (arg)
                        {
                            case "NOT READY":
                                if (OnSimPlugOut != null) ThreadPool.QueueUserWorkItem((o) => OnSimPlugOut?.Invoke());
                                break;

                            case "READY":
                                if (OnSimPlugIn != null) ThreadPool.QueueUserWorkItem((o) => OnSimPlugIn?.Invoke());
                                break;
                        }
                        break;
                    }

                case "CLIP":
                    {
                        if (OnCallingClip != null)
                            ThreadPool.QueueUserWorkItem((o) => OnCallingClip?.Invoke(new AnswerCallHelper(gsmClient, this, obj.Arguments.FirstOrDefault()?.Trim('\"'))));
                        break;
                    }

                case "CREG":
                case "CGREG":
                case "CEREG":
                    {
                        var registrationState = CommandRequestNetworkRegistration.RegistrationState.Parse(obj);
                        if (registrationState != null)
                            _UpdateRegistration(obj.Command, registrationState.IsRegistered);
                        break;
                    }
            }
        }

        /// <summary>
        /// Records one indicator and raises a connect/disconnect event only when the combined state
        /// flips, so a modem repeating the same URC stays quiet.
        /// </summary>
        /// <param name="indicator"></param>
        /// <param name="isRegistered"></param>
        void _UpdateRegistration(string indicator, bool isRegistered)
        {
            bool isProviderConnected;
            lock (_registrationIndicators)
            {
                _registrationIndicators[indicator] = isRegistered;
                isProviderConnected = _registrationIndicators.Values.Any(x => x);
                if (isProviderConnected == _isProviderConnected) return;
                _isProviderConnected = isProviderConnected;
            }

            if (isProviderConnected)
            {
                if (OnProviderConnected != null) ThreadPool.QueueUserWorkItem((o) => OnProviderConnected?.Invoke());
            }
            else
            {
                if (OnProviderDisconnected != null) ThreadPool.QueueUserWorkItem((o) => OnProviderDisconnected?.Invoke());
            }
        }

        private void GsmClient_OnUnknowReceived(string obj)
        {
            switch (obj)
            {
                case "RDY":
                    if (OnModuleReady != null) ThreadPool.QueueUserWorkItem((o) => OnModuleReady?.Invoke());
                    break;
                case "Call Ready":
                    //SIMCom only; Quectel and others never send it, which is why registration is
                    //also tracked through the +CREG/+CGREG/+CEREG indicators.
                    _UpdateRegistration(obj, true);
                    break;
                case "RING":
                    if (OnCalling != null) ThreadPool.QueueUserWorkItem((o) => OnCalling?.Invoke(new AnswerCallHelper(gsmClient, this)));
                    break;
                case "NO CARRIER":
                    if (OnEndCall != null) ThreadPool.QueueUserWorkItem((o) => OnEndCall?.Invoke());
                    break;
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public static class SimEventUtilsExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gsmClient"></param>
        /// <returns></returns>
        public static SimEventUtils RegisterSimEventUtils(this IGsmClient gsmClient) => new SimEventUtils(gsmClient);
    }
}
