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

namespace TqkLibrary.GSM
{
    //https://www.emnify.com/developer-blog/at-commands-for-cellular-modules
    public class GsmClient : IDisposable
    {
        static readonly IDictionary<int, string> _CME_Error = new Dictionary<int, string>()
        {
            { 0, "phone failure" },
            { 1, "no connection to phone" },
            { 2, "phone-adaptor link reserved" },
            { 3, "operation not allowed" },
            { 4, "operation not supported" },
            { 5, "PH-SIM PIN required" },
            { 10, "SIM not inserted" },
            { 11, "SIM PIN required" },
            { 12, "SIM PUK required" },
            { 13, "SIM failure" },
            { 14, "SIM busy" },
            { 15, "SIM wrong" },
            { 16, "incorrect password" },
            { 17, "SIM PIN2 required" },
            { 18, "SIM PUK2 required" },
            { 20, "memory full" },
            { 21, "invalid index" },
            { 22, "not found" },
            { 23, "memory failure" },
            { 24, "text string too long" },
            { 25, "invalid characters in text string" },
            { 26, "dial string too long" },
            { 27, "invalid characters in dial string" },
            { 30, "no network service" },
            { 31, "network timeout" },
            { 32, "network not allowed - emergency calls only" },
            { 40, "network personalization PIN required" },
            { 41, "network personalization PUK required" },
            { 42, "network subset personalization PIN required" },
            { 43, "network subset personalization PUK required" },
            { 44, "service provider personalization PIN required" },
            { 45, "service provider personalization PUK required" },
            { 46, "corporate personalization PIN required" },
            { 47, "corporate personalization PUK required" },

            //Easy CAMERA® related errors
            { 50, "Camera not found" },
            { 51, "Camera Initialization Error" },
            { 52, "Camera not Supported" },
            { 53, "No Photo Taken" },
            { 54, "NET BUSY...Camera TimeOut" },
            { 55, "Camera Error" },

            //General purpose error:
            { 100, "unknow" },
            
            //GPRS related errors to a failure to perform an Attach:
            { 103, "Illegal MS (#3)*" },
            { 106, "Illegal ME (#6)*" },
            { 107, "GPRS services not allowed" },
            { 111, "PLMN not allowed" },
            { 112, "Location Area not allowed" },
            { 113, "Roaming not allowed in this location area" },

            //GPRS related errors to a failure to Activate a Context and others
            { 132, "service option not supported" },
            { 133, "requested service option not subscribed" },
            { 134, "service option temporarily out of order" },
            { 148, "unspecified GPRS error" },
            { 149, "PDP authentication failure" },
            { 150, "invalid mobile class" },
            
            //Network survey error
            { 257, "Network survey error (No Carrier)*" },
            { 258, "Network survey error (Busy)*" },
            { 259, "Network survey error (Wrong request)*" },
            { 260, "Network survey error (Aborted)*" },

            //Easy GPRS® related errors
            { 400, "generic undocumented error" },
            { 401, "wrong state" },
            { 402, "wrong mode" },
            { 403, "context already activated" },
            { 404, "stack already active" },
            { 405, "activation failed" },
            { 406, "context not opened" },
            { 407, "cannot setup socket" },
            { 408, "cannot resolve DN" },
            { 409, "timeout in opening socket" },
            { 410, "cannot open socket" },
            { 411, "remote disconnected or timeout" },
            { 412, "connection failed" },
            { 413, "tx error" },
            { 414, "already listening" },

        };
        static readonly IDictionary<int, string> _CMS_Error = new Dictionary<int, string>()
        {

        };

        public string Port { get; }
        public bool IsOpen => serialPort.IsOpen;
        public event Action<string> LogCallback;
        /// <summary>
        /// default 20000ms
        /// </summary>
        public int CommandTimeout { get; set; } = 20000;
        readonly SerialPort serialPort;
        readonly AsyncLock asyncLockSend = new AsyncLock();
        public GsmClient(string port, int baudRate = 115200)
        {
            if (string.IsNullOrWhiteSpace(port)) throw new ArgumentNullException(nameof(port));
            serialPort = new SerialPort(port, baudRate, Parity.None, 8, StopBits.One);
            serialPort.DataReceived += SerialPort_DataReceived;
            serialPort.RtsEnable = true;
            serialPort.DtrEnable = false;
            this.Port = port;
        }

        ~GsmClient()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        void Dispose(bool disposing)
        {
            serialPort.Dispose();
        }

        public void Open() => serialPort.Open();

        /// <summary>
        /// true is OK, else false is ERROR
        /// </summary>
        public event Action<bool> OnCommandResult;
        /// <summary>
        /// +[Command]: [arg0],[arg1],[arg2],....\r\n[data]
        /// </summary>
        public event Action<GsmCommandResponse> OnCommandResponse;
        /// <summary>
        /// raw text reponse
        /// </summary>
        public event Action<string> OnUnknowReceived;
        /// <summary>
        /// +CME ERROR: - ME Error Result Code
        /// </summary>
        public event Action<string, int> OnMeError;
        /// <summary>
        /// +CMS ERROR - Message Service Failure Result Code
        /// </summary>
        public event Action<string, int> OnMsError;

        private static readonly Regex regex_splitResponse = new Regex(@"(?<=^\r?\n|[\x01-\x7E]\r?\n\r?\n)([\x01-\x7E]*?)(?=\r?\n\r?\n[\x01-\x7E]|\r?\n$)");

        private string temp = string.Empty;


        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] buffer = new byte[serialPort.ReadBufferSize];
            int byteRead = serialPort.Read(buffer, 0, buffer.Length);
            string received = temp + Encoding.UTF8.GetString(buffer, 0, byteRead);

            if (received.EndsWith("\r\n"))
            {
                temp = string.Empty;
                var matches = regex_splitResponse.Matches(received);
                foreach (Match match in matches)
                {
                    var matchString = match.Groups[1].Value;

                    string log = $"{Port} >> {matchString.Trim()}";
#if DEBUG
                    Console.WriteLine(log);
#endif
                    if (LogCallback != null)
                    {
                        ThreadPool.QueueUserWorkItem((o) => LogCallback?.Invoke(log));
                    }

                    if (matchString.StartsWith("OK"))
                    {
                        OnCommandResult?.Invoke(true);
                        continue;
                    }
                    if (matchString.StartsWith("ERROR"))
                    {
                        OnCommandResult?.Invoke(false);
                        continue;
                    }

                    if (matchString.StartsWith("+"))
                    {
                        const string cme_err = "+CME ERROR:";
                        if (matchString.StartsWith(cme_err))
                        {
                            string num = matchString.Substring(cme_err.Length).Trim();
                            if (int.TryParse(num, out int n))
                            {
                                string err_msg = string.Empty;
                                if (_CME_Error.ContainsKey(n)) err_msg = $"{_CME_Error[n]} ({n})";
                                else err_msg = n.ToString();
                                OnMeError?.Invoke(err_msg, n);
                                continue;
                            }
                        }

                        const string cms_err = "+CMS ERROR:";
                        if (matchString.StartsWith(cms_err))
                        {
                            string num = matchString.Substring(cms_err.Length).Trim();
                            if (int.TryParse(num, out int n))
                            {
                                string err_msg = string.Empty;
                                if (_CMS_Error.ContainsKey(n)) err_msg = $"{_CMS_Error[n]} ({n})";
                                else err_msg = n.ToString();
                                OnMsError?.Invoke(err_msg, n);
                                continue;
                            }
                        }

                        GsmCommandResponse gsmCommandResponse = GsmCommandResponse.Parse(matchString);
                        if (gsmCommandResponse != null)
                        {
#if DEBUG
                            Console.WriteLine($"------\tCommand:{gsmCommandResponse.Command}");
                            Console.WriteLine($"------\tArgs:[{string.Join(" , ", gsmCommandResponse.Arguments.Select(x => $"\"{x}\""))}]");
                            Console.WriteLine($"------\tOptions:[{string.Join(" , ", gsmCommandResponse.Options.Select(x => $"[{string.Join(" , ", x.Select(y => $"\"{y}\""))}]"))}]");
                            Console.WriteLine($"------\tData:{gsmCommandResponse.Data}");
#endif

                            OnCommandResponse?.Invoke(gsmCommandResponse);
                            continue;
                        }
                    }

                    OnUnknowReceived?.Invoke(matchString);
                }
            }
            else
            {
                temp = received;
            }
        }


        private async Task<GsmCommandResult> SendCommandAsync(string command, CancellationToken cancellationToken = default)
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
                using var register2 = cancellationTokenSource.Token.Register(() => tcs.TrySetException(new GsmCommandTimeoutException()));
                try
                {
                    OnCommandResult += action_ok;
                    OnMeError += action_me_err;
                    OnMsError += action_ms_err;
                    OnCommandResponse += action_commandResponse;
                    OnUnknowReceived += action_unknow;

                    string log = $"{Port} << {command.Trim()}";
#if DEBUG
                    Console.WriteLine(log);
#endif
                    if (LogCallback != null)
                    {
                        ThreadPool.QueueUserWorkItem((o) => LogCallback?.Invoke(log));
                    }

                    serialPort.Write(command);

                    await tcs.Task.ConfigureAwait(false);
                    gsmCommandResult.IsSuccess = tcs.Task.Result;
                    return gsmCommandResult;
                }
                finally
                {
                    OnCommandResult -= action_ok;
                    OnMeError -= action_me_err;
                    OnMsError -= action_ms_err;
                    OnCommandResponse -= action_commandResponse;
                    OnUnknowReceived -= action_unknow;
                }
            }
        }

#if DEBUG
        public void TestSend()
        {
            while (true)
            {
                string command = Console.ReadLine();
                serialPort.Write($"AT{command}\r\n");
            }
        }
#endif

        /// <summary>
        /// &lt;command&gt;=?
        /// </summary>
        /// <returns></returns>
        public Task<GsmCommandResult> TestAsync(string command, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(command)) throw new ArgumentNullException(nameof(command));
            return SendCommandAsync($"AT{command}=?\r\n", cancellationToken);
        }


        /// <summary>
        /// +&lt;command&gt;?
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Task<GsmCommandResult> ReadAsync(string command, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(command)) throw new ArgumentNullException(nameof(command));
            return SendCommandAsync($"AT+{command}?\r\n", cancellationToken);
        }


        /// <summary>
        /// +&lt;command&gt;=[val1],[val2],....
        /// </summary>
        /// <returns></returns>
        public Task<GsmCommandResult> WriteAsync(string command, CancellationToken cancellationToken = default, params object[] values)
            => WriteAsync(command, string.Join(",", values), cancellationToken);

        /// <summary>
        /// +&lt;command&gt;=[val1],[val2],....
        /// </summary>
        /// <returns></returns>
        public Task<GsmCommandResult> WriteAsync(string command, string[] values, CancellationToken cancellationToken = default)
            => WriteAsync(command, string.Join(",", values), cancellationToken);

        /// <summary>
        /// +&lt;command&gt;=[val1],[val2],....
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Task<GsmCommandResult> WriteAsync(string command, string value, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(command)) throw new ArgumentNullException(nameof(command));
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(nameof(value));
            return SendCommandAsync($"AT+{command}={value}\r\n", cancellationToken);
        }


        /// <summary>
        /// +&lt;command&gt;
        /// </summary>
        /// <returns></returns>
        public Task<GsmCommandResult> ExecuteAsync(string command, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(command)) throw new ArgumentNullException(nameof(command));
            return SendCommandAsync($"AT+{command}\r\n", cancellationToken);
        }

    }
}