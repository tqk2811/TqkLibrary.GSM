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

namespace GsmAtWrapper
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


        const int baudRate = 115200;
        readonly SerialPort serialPort;
        readonly AsyncLock asyncLockSend = new AsyncLock();
        public GsmClient(string port)
        {
            serialPort = new SerialPort(port, baudRate, Parity.None, 8, StopBits.One);
            serialPort.DataReceived += SerialPort_DataReceived;
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
        /// +[Command]: [data],[data],[data],....
        /// </summary>
        public event Action<string, string[]> OnCommandResponse;
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
        private static readonly Regex regex_Command = new Regex("^\\+([A-z0-9]+):([\\x01-\\x7E]+)$", RegexOptions.Multiline);
        private static readonly Regex regex_csv_partent = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

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
#if DEBUG
                    Console.WriteLine($">>{matchString.Trim()}");
#endif
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
                        if (matchString.StartsWith("+CME ERROR:"))
                        {
                            string num = matchString.Substring(11).Trim();
                            if (int.TryParse(num, out int n))
                            {
                                string err_msg = string.Empty;
                                if (_CME_Error.ContainsKey(n)) err_msg = _CME_Error[n];
                                OnMeError?.Invoke(err_msg, n);
                                continue;
                            }
                        }

                        if (matchString.StartsWith("+CMS ERROR:"))
                        {
                            string num = matchString.Substring(11).Trim();
                            if (int.TryParse(num, out int n))
                            {
                                string err_msg = string.Empty;
                                if (_CMS_Error.ContainsKey(n)) err_msg = _CMS_Error[n];
                                OnMsError?.Invoke(err_msg, n);
                                continue;
                            }
                        }

                        Match match_cmd = regex_Command.Match(matchString);
                        if (match_cmd.Success)
                        {
                            string command = match_cmd.Groups[1].Value;
                            string values = match_cmd.Groups[2].Value.Trim();
                            string[] values_split = regex_csv_partent.Split(values);

                            OnCommandResponse?.Invoke(command, values_split);
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


        private async Task<GsmCommandResult> SendCommand(string command, CancellationToken cancellationToken = default)
        {
            using (await asyncLockSend.LockAsync(cancellationToken).ConfigureAwait(false))
            {
                GsmCommandResult gsmCommandResult = new GsmCommandResult();

                TaskCompletionSource<bool> tcs_ok = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                Action<bool> action_ok = (r) => tcs_ok.TrySetResult(r);
                Action<string, int> action_me_err = (msg, code) => tcs_ok.TrySetException(new MEException(code, msg));
                Action<string, int> action_ms_err = (msg, code) => tcs_ok.TrySetException(new MSException(code, msg));
                Action<string, string[]> action_commandResponse = (name, args) => gsmCommandResult._CommandResponses.Add(name, args);
                Action<string> action_unknow = (str) => gsmCommandResult._Datas.Add(str);

                using var register = cancellationToken.Register(() => tcs_ok.TrySetCanceled());
                try
                {
                    OnCommandResult += action_ok;
                    OnMeError += action_me_err;
                    OnMsError += action_ms_err;
                    OnCommandResponse += action_commandResponse;
                    OnUnknowReceived += action_unknow;

#if DEBUG
                    Console.WriteLine($"<<{command.Trim()}");
#endif
                    serialPort.Write(command);

                    await tcs_ok.Task.ConfigureAwait(false);
                    gsmCommandResult.IsSuccess = tcs_ok.Task.Result;
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


        public void TestSend()
        {
            while (true)
            {
                string command = Console.ReadLine();
                serialPort.Write($"AT{command}\r\n");
            }
        }


        // ?
        public async Task<GsmCommandResult> Test(string command, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(command)) throw new ArgumentNullException(nameof(command));
            return await SendCommand($"AT{command}=?\r\n").ConfigureAwait(false);
        }


        // + ?
        public async Task<GsmCommandResult> Read(string command, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(command)) throw new ArgumentNullException(nameof(command));
            return await SendCommand($"AT+{command}?\r\n").ConfigureAwait(false);
        }



        public Task<GsmCommandResult> Write(string command, CancellationToken cancellationToken = default, params object[] values)
            => Write(command, string.Join(",", values), cancellationToken);
        public Task<GsmCommandResult> Write(string command, string[] values, CancellationToken cancellationToken = default)
            => Write(command, string.Join(",", values), cancellationToken);
        // + = 
        public async Task<GsmCommandResult> Write(string command, string value, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(command)) throw new ArgumentNullException(nameof(command));
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(nameof(value));
            return await SendCommand($"AT+{command}={value}\r\n").ConfigureAwait(false);
        }


        // +
        public async Task<GsmCommandResult> Execute(string command, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(command)) throw new ArgumentNullException(nameof(command));
            return await SendCommand($"AT+{command}\r\n").ConfigureAwait(false);
        }

    }
}