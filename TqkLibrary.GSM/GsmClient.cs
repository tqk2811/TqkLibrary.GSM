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
    /// <summary>
    /// 
    /// </summary>
    public class GsmClient : IDisposable
    {
        /// <summary>
        /// for not break character >= 0x80 when convert back to byte
        /// </summary>
        internal static readonly Encoding GsmEncoding;
        static GsmClient()
        {
            GsmEncoding = Encoding.GetEncoding("ISO-8859-1");
        }
        static readonly IReadOnlyDictionary<int, string> _CME_Error = new Dictionary<int, string>()
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

            { 3801, "HTTP time out" },
            { 3802, "HTTP busy" },
            { 3803, "HTTP UART busy" },
            { 3804, "HTTP get no request" },
            { 3805, "HTTP network busy" },
            { 3806, "HTTP network open failed" },
            { 3807, "HTTP network no config" },
            { 3808, "HTTP network deactive" },
            { 3809, "HTTP network error" },
            { 3810, "HTTP url error" },
            { 3811, "HTTP empty url" },
            { 3812, "HTTP ip addr error" },
            { 3813, "HTTP DNS error" },
            { 3814, "HTTP socket create error" },
            { 3815, "HTTP socket connect error" },
            { 3816, "HTTP socket read error" },
            { 3817, "HTTP socket write error" },
            { 3818, "HTTP socket close" },
            { 3819, "HTTP data encode error" },
            { 3820, "HTTP data decode error" },
            { 3821, "HTTP to read timeout" },
            { 3822, "HTTP response failed" },
            { 3823, "incoming call busy" },
            { 3824, "voice call busy" },
            { 3825, "input timeout " },
            { 3826, "wait data timeout" },
            { 3827, "wait http response timeout" },
            { 3828, "alloc memory fail" },
            { 3829, "HTTP need relocation" },

            { 4000, "Exceed max length" },
            { 4001, "Open file fail" },
            { 4002, "Write file fail" },
            { 4003, "Get size fail" },
            { 4004, "Read fail" },
            { 4005, "List file fail" },
            { 4006, "Delete file fail" },
            { 4007, "Get Disk info fail" },
            { 4008, "No space" },
            { 4009, "Time out" },
            { 4010, "File not found" },
            { 4011, "File too large" },
            { 4012, "File already exist" },
            { 4013, "Invalid parameter" },
            { 4014, "Driver error" },
            { 4015, "Create fail" },
            { 4016, "access denied" },
            { 4017, "File too large" },
        };
        static readonly IReadOnlyDictionary<int, string> _CMS_Error = new Dictionary<int, string>()
        {

        };
        //CR
        const string LineBreak = "\r\n";

        /// <summary>
        /// 
        /// </summary>
        public string Port { get; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsOpen => serialPort.IsOpen;
        /// <summary>
        /// 
        /// </summary>
        public event Action<string> LogCallback;
        /// <summary>
        /// default 20000ms
        /// </summary>
        public int CommandTimeout { get; set; }
#if DEBUG
            = 20000000;
#else
            = 20000;
#endif

        readonly SerialPort serialPort;
        readonly AsyncLock asyncLockSend = new AsyncLock();
        readonly SynchronizationContext _synchronizationContext;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="baudRate"></param>
        /// <param name="synchronizationContext"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public GsmClient(string port, int baudRate = 115200, SynchronizationContext synchronizationContext = null)
        {
            if (string.IsNullOrWhiteSpace(port)) throw new ArgumentNullException(nameof(port));
            serialPort = new SerialPort(port, baudRate, Parity.None, 8, StopBits.One);
            serialPort.Handshake = Handshake.RequestToSend;
            serialPort.DataReceived += SerialPort_DataReceived;
            //serialPort.RtsEnable = true;
            //serialPort.DtrEnable = false;
            this.Port = port;
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
            serialPort.Dispose();
        }
        /// <summary>
        /// 
        /// </summary>
        public void Open() => serialPort.Open();

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

        private readonly byte[] _buffer = new byte[5 * 1024 * 1024];
        private int _bufferDataCount = 0;

        private static readonly Regex regex_response2
            = new Regex("(^\r\n\\+[\\x00-\\xFF]*?\r\n(?=\r\n)|\r\n\\+[\\x00-\\xFF]*?\r\n$|\r\n[\\x00-\\xFF]*?\r\n)", RegexOptions.Multiline);
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                int byteRead = serialPort.Read(_buffer, _bufferDataCount, serialPort.ReadBufferSize);
                _bufferDataCount += byteRead;

                if (_buffer.EndWith(_bufferDataCount, "\r\n"))
                {
                    string received = GsmEncoding.GetString(_buffer, 0, _bufferDataCount);

#if DEBUG
                    Console.WriteLine($"------\tReceived: {received.PrintCRLFHepler()}");
#endif
                    if (_buffer.StartWith("\r\n"))
                    {
                        //sone device not send StartWith AT<sender_command>
                        // \r\nOK\r\n    or      \r\nERROR\r\n      or       \r\n+CME ERROR: 4010\r\n     or       \r\n+CMS ERROR: 123\r\n
                        if (_FooterCheck(received))
                        {
                            _bufferDataCount = 0;//clear
                        }
                        else
                        {
                            // \r\n+CMT: ,33\r\n07914889200026F5240B914883537892F10008323012519243820E0053006D00730020006D1EAB0075\r\n
                            // \r\nRING\r\n
                            // \r\nRING\r\n\r\n+CLIP: "0383599999",129,"",,"",0\r\n
                            // \r\n+CRING: VOICE\r\n\r\n+CLIP: "0383599999",129,"",,"",0\r\n

                            // \r\n+CPMS: 0,40,0,10,0,10\r\n\r\nOK\r\n
                            // => some device not send AT+CPMS="SM"\r\r\n+CPMS: 0,40,0,10,0,10\r\n\r\nOK\r\n
                            if (!_MatchResult(received))
                            {
                                _bufferDataCount = 0;//clear
                                var matches = regex_response2.Matches(received);
                                foreach (Match item in matches)
                                {
                                    GsmCommandResponse gsmCommandResponse = GsmCommandResponse.Parse(item.Value);
                                    if (gsmCommandResponse is not null)
                                    {
                                        _BodyInvokeAndPrint(gsmCommandResponse);
                                    }
                                    else
                                    {
                                        _FireUnknowReceived(received.Trim());
                                    }
                                }
                                _WriteReceivedLog(received);
                            }
                        }
                    }
                    else if (_buffer.StartWith("AT"))
                    {
                        //Window1252 match failed
                        if (!_MatchResult(received))
                        {
#if DEBUG
                            Console.WriteLine($"------\ttregex_response Match failed: {received.PrintCRLFHepler()}");
#endif
                        }
                    }
#if DEBUG
                    else
                    {
                        Console.WriteLine($"------\tUnknow tReceived Type: {received.PrintCRLFHepler()}");
                    }
#endif
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine($"{ex.GetType().FullName}: {ex.Message}, {ex.StackTrace}");
#endif
            }
        }

        private static readonly Regex regex_response
            = new Regex("^(AT.*?\r|)(\r\n[\\x00-\\xFF]*?\r\n|)\r\n(OK|ERROR|\\+CM. ERROR:.*?)\r\n$", RegexOptions.Multiline);
        bool _MatchResult(string received)
        {
            var match = regex_response.Match(received);
            if (match.Success)//Make sure match OK|ERROR|\+CM. ERROR:.*? at the end
            {
                // 3 group: head body footer
                //Head: AT+QFDWL=\"RAM:sound.wav\"\r     or      AT+COPS?\r       or      <empty>
                //Body: <empty>     or   \r\n+COPS: 0,0,\"VINAPHONE\"\r\n      or       \r\nCONNECT\r\n\xab\xff\x34...long binary data....\xac\r\n+QFDWL: 20,3\r\n
                //Footer: \r\nOK\r\n    or      \r\nERROR\r\n      or       \r\n+CME ERROR: 4010\r\n     or       \r\n+CMS ERROR: 123\r\n
                string head = match.Groups[1].Value;
                string body = match.Groups[2].Value;
                string footer = match.Groups[3].Value.Trim();
                List<string> logs = new List<string>();

                if (!string.IsNullOrWhiteSpace(head.Trim())) logs.Add(head.Trim());

                if (!string.IsNullOrWhiteSpace(body.Trim()))
                {
                    if (body.StartsWith("\r\n+") ||// +COPS:
                        body.StartsWith("\r\nCONNECT\r\n"))//data mode     \r\nCONNECT\r\n
                    {
                        GsmCommandResponse gsmCommandResponse = _BodyParse(body);
                        if (gsmCommandResponse is not null && body.StartsWith("\r\nCONNECT\r\n"))
                        {
                            logs.Add($"CONNECT\r\n[binarySize={gsmCommandResponse.BinaryData?.Count()}]\r\n+{gsmCommandResponse.Command}: {string.Join(",", gsmCommandResponse.Arguments)}");
                        }
                        else
                        {
                            logs.Add(body.Trim());
                        }
                    }
#if DEBUG
                    else
                    {
                        Console.WriteLine($"------\tUnknow Body Type: {match.Value.PrintCRLFHepler()}");
                    }
#endif
                }

                logs.Add(footer.Trim());


                if (_FooterCheck(footer, false))
                {
                    _WriteReceivedLog(string.Join("\r\n", logs));
                }
#if DEBUG
                else
                {
                    Console.WriteLine($"------\tUnknow _MatchResult Type: {match.Value.PrintCRLFHepler()}");
                }
#endif

                if (match.Value.Length < _bufferDataCount)
                {
                    for (int i = 0; i < _bufferDataCount - match.Value.Length; i += match.Value.Length)
                    {
                        Array.Copy(_buffer, i, _buffer, match.Value.Length + i, Math.Min(match.Value.Length, _bufferDataCount - match.Value.Length));
                    }
                    _bufferDataCount -= match.Value.Length;
                }
                else
                {
                    _bufferDataCount = 0;
                }
            }
            return match.Success;
        }

        bool _FooterCheck(string footer, bool isWriteReceivedLog = true)
        {
            footer = footer.Trim();
            switch (footer)
            {
                case "OK":
                    _FireCommandResult(true);
                    if (isWriteReceivedLog) _WriteReceivedLog(footer);
                    return true;

                case "ERROR":
                    _FireCommandResult(false);
                    if (isWriteReceivedLog) _WriteReceivedLog(footer);
                    return true;

                default:
                    {
                        const string cme_err = "+CME ERROR:";
                        const string cms_err = "+CMS ERROR:";
                        if (footer.StartsWith(cme_err))
                        {
                            string num = footer.Substring(cme_err.Length).Trim();
                            if (int.TryParse(num, out int n))
                            {
                                string err_msg = string.Empty;
                                if (_CME_Error.ContainsKey(n)) err_msg = $"{_CME_Error[n]} ({n})";
                                else err_msg = n.ToString();
                                _FireMeError(err_msg, n);
                                if (isWriteReceivedLog) _WriteReceivedLog(footer);
                                return true;
                            }
                        }
                        else if (footer.StartsWith(cms_err))
                        {
                            string num = footer.Substring(cms_err.Length).Trim();
                            if (int.TryParse(num, out int n))
                            {
                                string err_msg = string.Empty;
                                if (_CMS_Error.ContainsKey(n)) err_msg = $"{_CMS_Error[n]} ({n})";
                                else err_msg = n.ToString();
                                _FireMsError(err_msg, n);
                                if (isWriteReceivedLog) _WriteReceivedLog(footer);
                                return true;
                            }
                        }
                    }
                    return false;
            }
        }

        GsmCommandResponse _BodyParse(string body_text)
        {
            GsmCommandResponse gsmCommandResponse = GsmCommandResponse.Parse(body_text);
            if (gsmCommandResponse != null)
            {
                _BodyInvokeAndPrint(gsmCommandResponse);
            }
#if DEBUG
            else
            {
                Console.WriteLine($"------\tGsmCommandResponse.Parse Error: {body_text.PrintCRLFHepler()}");
            }
#endif
            return gsmCommandResponse;
        }
        void _BodyInvokeAndPrint(GsmCommandResponse gsmCommandResponse)
        {
            if (gsmCommandResponse != null)
            {
#if DEBUG
                Console.WriteLine($"------\tCommand:{gsmCommandResponse.Command.PrintCRLFHepler()}");
                Console.WriteLine($"------\tArgs:[{string.Join(" , ", gsmCommandResponse.Arguments.Select(x => $"{x.PrintCRLFHepler()}"))}]");
                Console.WriteLine($"------\tOptions:[{string.Join(" , ", gsmCommandResponse.Options.Select(x => $"[{string.Join(" , ", x.Select(y => $"\"{y}\""))}]"))}]");
                Console.WriteLine($"------\tData:{gsmCommandResponse.Data.PrintCRLFHepler()}");
#endif
                _FireCommandResponse(gsmCommandResponse);
            }
        }

        void _WriteReceivedLog(string log)
        {
            if (LogCallback != null)
            {
                string _log = $"{Port} >> {log.Trim()}";
                ThreadPool.QueueUserWorkItem((o) => LogCallback?.Invoke(_log));
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
                    Console.WriteLine($"{Port} << {command.PrintCRLFHepler()}");
#endif
                    if (LogCallback != null)
                    {
                        string log = $"{Port} << {command}";
                        ThreadPool.QueueUserWorkItem((o) => LogCallback?.Invoke(log));
                    }

                    serialPort.Write(command);

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
            serialPort.Write($"AT{command}{LineBreak}");
        }


        /// <summary>
        /// &lt;command&gt;=?
        /// </summary>
        /// <returns></returns>
        public Task<GsmCommandResult> TestAsync(string command, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(command)) throw new ArgumentNullException(nameof(command));
            return SendCommandAsync($"AT{command}=?{LineBreak}", cancellationToken);
        }


        /// <summary>
        /// +&lt;command&gt;?
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Task<GsmCommandResult> ReadAsync(string command, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(command)) throw new ArgumentNullException(nameof(command));
            return SendCommandAsync($"AT+{command}?{LineBreak}", cancellationToken);
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
            return SendCommandAsync($"AT+{command}={value}{LineBreak}", cancellationToken);
        }


        /// <summary>
        /// +&lt;command&gt;
        /// </summary>
        /// <returns></returns>
        public Task<GsmCommandResult> ExecuteAsync(string command, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(command)) throw new ArgumentNullException(nameof(command));
            return SendCommandAsync($"AT+{command}{LineBreak}", cancellationToken);
        }

    }
}