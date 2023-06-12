using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TqkLibrary.GSM.Interfaces;

namespace TqkLibrary.GSM.AtClient
{
    /// <summary>
    /// 
    /// </summary>
    public class AtClientParse : IAtClient
    {
        /// <summary>
        /// for not break character >= 0x80 when convert back to byte
        /// </summary>
        internal static readonly Encoding GsmEncoding;
        static AtClientParse()
        {
            GsmEncoding = Encoding.GetEncoding("ISO-8859-1");
        }


        readonly SerialPort serialPort;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="baudRate"></param>
        public AtClientParse(string port, int baudRate = 115200)
        {
            if (string.IsNullOrWhiteSpace(port)) throw new ArgumentNullException(nameof(port));
            serialPort = new SerialPort(port, baudRate, Parity.None, 8, StopBits.One);
            serialPort.Handshake = Handshake.RequestToSend;
            serialPort.DataReceived += SerialPort_DataReceived;
        }
        /// <summary>
        /// 
        /// </summary>
        ~AtClientParse()
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
        public bool IsOpen => serialPort.IsOpen;
        /// <summary>
        /// 
        /// </summary>
        public string PortName => serialPort.PortName;
        /// <summary>
        /// 
        /// </summary>
        public event Action<bool> OnCommandResult;
        /// <summary>
        /// 
        /// </summary>
        public event Action<GsmCommandResponse> OnCommandResponse;
        /// <summary>
        /// 
        /// </summary>
        public event Action<string> OnUnknowReceived;
        /// <summary>
        /// 
        /// </summary>
        public event Action<string, int> OnMeError;
        /// <summary>
        /// 
        /// </summary>
        public event Action<string, int> OnMsError;
        /// <summary>
        /// 
        /// </summary>
        public event Action<string> OnLogCallback;


        private readonly byte[] _buffer = new byte[5 * 1024 * 1024];
        private int _bufferDataCount = 0;
        private static readonly Regex regex_response2
            = new Regex("(^\r\n\\+[\\x00-\\xFF]*?\r\n(?=\r\n)|\r\n\\+[\\x00-\\xFF]*?\r\n$|\r\n[\\x00-\\xFF]*?\r\n)", RegexOptions.Multiline);
        private static readonly Regex regex_response
            = new Regex("^(AT.*?\r|)(\r\n[\\x00-\\xFF]*?\r\n|)\r\n(OK|ERROR|\\+CM. ERROR:.*?)\r\n$", RegexOptions.Multiline);
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
                                    GsmCommandResponse gsmCommandResponse = ParseGsmCommandResponse(item.Value);
                                    if (gsmCommandResponse is not null)
                                    {
                                        _BodyInvokeAndPrint(gsmCommandResponse);
                                    }
                                    else
                                    {
                                        OnUnknowReceived?.Invoke(received.Trim());
                                    }
                                }
                                _WriteReceivedLog(received, "!_MatchResult");
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
                        if (body.StartsWith("\r\nCONNECT\r\n"))
                        {
                            logs.Add($"CONNECT\r\n[binarySize={gsmCommandResponse?.BinaryData?.Count()}]\r\n+{gsmCommandResponse?.Command}: {string.Join(",", gsmCommandResponse?.Arguments ?? new string[0])}");

                            if (gsmCommandResponse is null)
                            {
                                logs.Add($"[ParseBodyFailed]: \r\n\tFirst: {body.Substring(0, 30)}\r\n\tLast: {body.Substring(body.Length - 30)}");
                            }
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
                    _WriteReceivedLog(string.Join("\r\n", logs), nameof(_MatchResult).Trim('_'));
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
                    OnCommandResult?.Invoke(true);
                    if (isWriteReceivedLog) _WriteReceivedLog(footer, nameof(_FooterCheck).Trim('_'));
                    return true;

                case "ERROR":
                    OnCommandResult?.Invoke(false);
                    if (isWriteReceivedLog) _WriteReceivedLog(footer, nameof(_FooterCheck).Trim('_'));
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
                                if (Consts.CME_Error.ContainsKey(n)) err_msg = $"{Consts.CME_Error[n]} ({n})";
                                else err_msg = n.ToString();
                                OnMeError?.Invoke(err_msg, n);
                                if (isWriteReceivedLog) _WriteReceivedLog(footer, nameof(_FooterCheck).Trim('_'));
                                return true;
                            }
                        }
                        else if (footer.StartsWith(cms_err))
                        {
                            string num = footer.Substring(cms_err.Length).Trim();
                            if (int.TryParse(num, out int n))
                            {
                                string err_msg = string.Empty;
                                if (Consts.CMS_Error.ContainsKey(n)) err_msg = $"{Consts.CMS_Error[n]} ({n})";
                                else err_msg = n.ToString();
                                OnMsError?.Invoke(err_msg, n);
                                if (isWriteReceivedLog) _WriteReceivedLog(footer, nameof(_FooterCheck).Trim('_'));
                                return true;
                            }
                        }
                    }
                    return false;
            }
        }

        GsmCommandResponse _BodyParse(string body_text)
        {
            GsmCommandResponse gsmCommandResponse = ParseGsmCommandResponse(body_text);
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
                OnCommandResponse?.Invoke(gsmCommandResponse);
            }
        }

        void _WriteReceivedLog(string log, string name = "")
        {
            if (OnLogCallback != null)
            {
                string _log = string.IsNullOrWhiteSpace(name) ? string.Empty : $"[{name}]";
                _log += $"{PortName} >> {log.Trim()}";
                OnLogCallback?.Invoke(_log);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        public void Write(string command)
        {
            serialPort.Write(command);
        }
        /// <summary>
        /// 
        /// </summary>
        public void Open()
        {
            serialPort.Open();
        }



        private static readonly Regex regex_Command
            = new Regex("^(CONNECT\r\n[\\x00-\\xFF]*?\r\n|)\\+([A-z0-9]+):([\\x01-\\x0C\\x0E-\\x7E]+)(|\\r\\n[\\x01-\\x7E]+)$", RegexOptions.Multiline);
        private static readonly Regex regex_csv_pattern
            = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static GsmCommandResponse ParseGsmCommandResponse(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return null;
            str = str.Trim();
            Match match = regex_Command.Match(str);
            if (match.Success)
            {
                string connect_binary_str = match.Groups[1].Value;
                byte[] binary = null;
                if (!string.IsNullOrWhiteSpace(connect_binary_str))
                {
                    binary = GsmEncoding.GetBytes(connect_binary_str)
                        .Skip(9)// CONNECT\r\n    length
                        .SkipLast(2)//\r\n
                        .ToArray();
                }

                string command = match.Groups[2].Value.Trim();
                string args = match.Groups[3].Value.Trim();
                if (args.StartsWith("(") && args.EndsWith(")"))
                {
                    string[] options_split = Regex.Split(args.Substring(1, args.Length - 2), "\\),\\(");
                    return new GsmCommandResponse(command, options_split.Select(x => regex_csv_pattern.Split(x)).ToList());
                }
                else
                {
                    string data = match.Groups[4].Value.Trim();
                    string[] args_split = regex_csv_pattern.Split(args);
                    return new GsmCommandResponse(command, args_split, data, binary);
                }
            }
            return null;
        }
    }
}
