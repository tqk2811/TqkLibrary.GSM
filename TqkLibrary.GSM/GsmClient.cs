using System.IO.Ports;
using System.IO;
using Nito.AsyncEx;
using System.Threading;
using System.Text;
using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using TqkLibrary.GSM.Exceptions;
using System.Reflection;
using TqkLibrary.GSM.AtClient;
using TqkLibrary.GSM.Interfaces;

namespace TqkLibrary.GSM
{
    //https://www.emnify.com/developer-blog/at-commands-for-cellular-modules
    /// <summary>
    /// 
    /// </summary>
    public class GsmClient : IGsmClient
    {
        /// <summary>
        /// 
        /// </summary>
        public string PortName => atClient.PortName;
        /// <summary>
        /// 
        /// </summary>
        public bool IsOpen => atClient.IsOpen;
        /// <summary>
        /// 
        /// </summary>
        public event Action<string> OnLogCallback;
        /// <summary>
        /// default 20000ms
        /// </summary>
        public int CommandTimeout { get; set; } = Consts.CommandTimeout;

        readonly IAtClient atClient;

        readonly AsyncLock asyncLockSend = new AsyncLock();
        readonly SynchronizationContext _synchronizationContext;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="atClient"></param>
        /// <param name="synchronizationContext"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public GsmClient(IAtClient atClient, SynchronizationContext synchronizationContext = null)
        {
            this.atClient = atClient ?? throw new ArgumentNullException(nameof(atClient));
            atClient.OnLogCallback += (l) => OnLogCallback?.Invoke(l);
            atClient.OnCommandResponse += _FireCommandResponse;
            atClient.OnUnknowReceived += _FireUnknowReceived;
            atClient.OnCommandResult += _FireCommandResult;
            atClient.OnMsError += _FireMsError;
            atClient.OnMeError += _FireMeError;
            atClient.OnConnectDataEvent += _FireConnectDataEvent;
            atClient.OnPromptEvent += _FirePromptEvent;

            this._synchronizationContext = synchronizationContext;
        }


        /// <summary>
        /// 
        /// </summary>
        ~GsmClient()
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
        void Dispose(bool disposing)
        {
            atClient.Dispose();
        }
        /// <summary>
        /// 
        /// </summary>
        public void Open() => atClient.Open();

        /// <summary>
        /// true is OK, else false is ERROR
        /// </summary>
        public event Action<bool> OnCommandResult;
        event Action<bool> _OnCommandResult;
        /// <summary>
        /// +[Command]: [arg0],[arg1],[arg2],....\r\n[data]
        /// </summary>
        public event Action<GsmCommandResponse> OnCommandResponse;
        event Action<GsmCommandResponse> _OnCommandResponse;
        /// <summary>
        /// raw text reponse
        /// </summary>
        public event Action<string> OnUnknowReceived;
        event Action<string> _OnUnknowReceived;
        /// <summary>
        /// +CME ERROR: - ME Error Result Code
        /// </summary>
        public event Action<string, int> OnMeError;
        event Action<string, int> _OnMeError;
        /// <summary>
        /// +CMS ERROR - Message Service Failure Result Code
        /// </summary>
        public event Action<string, int> OnMsError;
        event Action<string, int> _OnMsError;
        /// <summary>
        /// 
        /// </summary>
        public event Action<ConnectDataEvent> OnConnectDataEvent;
        event Action<ConnectDataEvent> _OnConnectDataEvent;
        /// <summary>
        /// 
        /// </summary>
        public event Action<PromptEvent> OnPromptEvent;
        event Action<PromptEvent> _OnPromptEvent;

        void _FireCommandResult(bool result)
        {
            _OnCommandResult?.Invoke(result);
            if (OnCommandResult is not null)
            {
                if (_synchronizationContext is not null)
                {
                    _synchronizationContext.Post((o) => OnCommandResult?.Invoke(result), null);
                }
                else
                {
                    ThreadPool.QueueUserWorkItem((o) => OnCommandResult?.Invoke(result), null);
                }
            }

        }
        void _FireCommandResponse(GsmCommandResponse commandResponse)
        {
            _OnCommandResponse?.Invoke(commandResponse);
            if (OnCommandResponse is not null)
            {
                if (_synchronizationContext is not null)
                {
                    _synchronizationContext.Post((o) => OnCommandResponse?.Invoke(commandResponse), null);
                }
                else
                {
                    ThreadPool.QueueUserWorkItem((o) => OnCommandResponse?.Invoke(commandResponse), null);
                }
            }
        }
        void _FireUnknowReceived(string text)
        {
            _OnUnknowReceived?.Invoke(text);
            if (OnUnknowReceived is not null)
            {
                if (_synchronizationContext is not null)
                {
                    _synchronizationContext.Post((o) => OnUnknowReceived?.Invoke(text), null);
                }
                else
                {
                    ThreadPool.QueueUserWorkItem((o) => OnUnknowReceived?.Invoke(text), null);
                }
            }
        }
        void _FireMeError(string message, int code)
        {
            _OnMeError?.Invoke(message, code);
            if (OnMeError is not null)
            {
                if (_synchronizationContext is not null)
                {
                    _synchronizationContext.Post((o) => OnMeError?.Invoke(message, code), null);
                }
                else
                {
                    ThreadPool.QueueUserWorkItem((o) => OnMeError?.Invoke(message, code), null);
                }
            }
        }
        void _FireMsError(string message, int code)
        {
            _OnMsError?.Invoke(message, code);
            if (OnMsError is not null)
            {
                if (_synchronizationContext is not null)
                {
                    _synchronizationContext.Post((o) => OnMsError?.Invoke(message, code), null);
                }
                else
                {
                    ThreadPool.QueueUserWorkItem((o) => OnMsError?.Invoke(message, code), null);
                }
            }
        }
        void _FireConnectDataEvent(ConnectDataEvent connectDataEvent)
        {
            _OnConnectDataEvent?.Invoke(connectDataEvent);
            if (OnConnectDataEvent is not null)
            {
                if (_synchronizationContext is not null)
                {
                    _synchronizationContext.Post((o) => OnConnectDataEvent?.Invoke(connectDataEvent), null);
                }
                else
                {
                    ThreadPool.QueueUserWorkItem((o) => OnConnectDataEvent?.Invoke(connectDataEvent), null);
                }
            }
        }
        void _FirePromptEvent(PromptEvent promptEvent)
        {
            _OnPromptEvent?.Invoke(promptEvent);
            if (OnPromptEvent is not null)
            {
                if (_synchronizationContext is not null)
                {
                    _synchronizationContext.Post((o) => OnPromptEvent?.Invoke(promptEvent), null);
                }
                else
                {
                    ThreadPool.QueueUserWorkItem((o) => OnPromptEvent?.Invoke(promptEvent), null);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<GsmCommandResult> SendCommandAsync(string command, CancellationToken cancellationToken = default)
        {
            using (await asyncLockSend.LockAsync(cancellationToken).ConfigureAwait(false))
            {
                GsmCommandResult gsmCommandResult = new GsmCommandResult();

                TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                Action<bool> action_ok = (r) => tcs.TrySetResult(r);
                Action<string, int> action_me_err = (msg, code) => tcs.TrySetException(new MEException(code, msg));
                Action<string, int> action_ms_err = (msg, code) => tcs.TrySetException(new MSException(code, msg));
                Action<GsmCommandResponse> action_commandResponse = (commandData) => gsmCommandResult._CommandResponses.Add(commandData);
                Action<string> action_unknow = (str) => gsmCommandResult._Datas.Add(str);

                using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(CommandTimeout);
                using var register = cancellationToken.Register(() => tcs.TrySetCanceled());
                using var register2 = cancellationTokenSource.Token.Register(() => tcs.TrySetException(new GsmCommandTimeoutException(command)));
                try
                {
                    _OnCommandResult += action_ok;
                    _OnMeError += action_me_err;
                    _OnMsError += action_ms_err;
                    _OnCommandResponse += action_commandResponse;
                    _OnUnknowReceived += action_unknow;

#if DEBUG
                    Console.WriteLine($"{PortName} << {command.PrintCRLFHepler()}");
#endif
                    if (OnLogCallback != null)
                    {
                        string log = $"{PortName} << {command}";
                        ThreadPool.QueueUserWorkItem((o) => OnLogCallback?.Invoke(log));
                    }

                    atClient.Write(command);

                    await tcs.Task.ConfigureAwait(false);
                    gsmCommandResult.IsSuccess = tcs.Task.Result;
                    return gsmCommandResult;
                }
                finally
                {
                    _OnCommandResult -= action_ok;
                    _OnMeError -= action_me_err;
                    _OnMsError -= action_ms_err;
                    _OnCommandResponse -= action_commandResponse;
                    _OnUnknowReceived -= action_unknow;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        public void Debug(string command)
        {
            atClient.Write($"AT{command}{Consts.LineBreak}");
        }
    }
}