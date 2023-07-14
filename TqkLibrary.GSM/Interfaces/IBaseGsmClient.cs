namespace TqkLibrary.GSM.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBaseGsmClient : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        bool IsOpen { get; }
        /// <summary>
        /// 
        /// </summary>
        string PortName { get; }
        /// <summary>
        /// true is OK, else false is ERROR
        /// </summary>
        event Action<bool> OnCommandResult;
        /// <summary>
        /// +[Command]: [arg0],[arg1],[arg2],....\r\n[data]
        /// </summary>
        event Action<GsmCommandResponse> OnCommandResponse;
        /// <summary>
        /// raw text reponse
        /// </summary>
        event Action<string> OnUnknowReceived;
        /// <summary>
        /// +CME ERROR: - ME Error Result Code
        /// </summary>
        event Action<string, int> OnMeError;
        /// <summary>
        /// +CMS ERROR - Message Service Failure Result Code
        /// </summary>
        event Action<string, int> OnMsError;
        /// <summary>
        /// 
        /// </summary>
        event Action<ConnectDataEvent> OnConnectDataEvent;
        /// <summary>
        /// 
        /// </summary>
        event Action<PromptEvent> OnPromptEvent;

        /// <summary>
        /// 
        /// </summary>
        event Action<string> OnLogCallback;

        /// <summary>
        /// 
        /// </summary>
        void Open();
    }
}
