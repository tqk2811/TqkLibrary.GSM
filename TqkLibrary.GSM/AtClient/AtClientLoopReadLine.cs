using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.IO;
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
    public class AtClientLoopReadLine : IAtClient
    {
        readonly SerialPort _serialPort;
        readonly SynchronizationContext _synchronizationContext;
        /// <summary>
        /// 
        /// </summary>
        public AtClientLoopReadLine(string port, int baudRate = 115200, SynchronizationContext synchronizationContext = null)
        {
            if (string.IsNullOrWhiteSpace(port)) throw new ArgumentNullException(nameof(port));
            _synchronizationContext = synchronizationContext;
            _serialPort = new SerialPort(port, baudRate, Parity.None, 8, StopBits.One);
            _serialPort.Encoding = Consts.ISO8859;
            _serialPort.NewLine = Consts.LineBreak;
            _serialPort.Handshake = Handshake.RequestToSend;
            //_serialPort.DataReceived += SerialPort_DataReceived;
        }
        /// <summary>
        /// 
        /// </summary>
        ~AtClientLoopReadLine()
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
            _serialPort.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsOpen => _serialPort.IsOpen;
        /// <summary>
        /// 
        /// </summary>
        public string PortName => _serialPort.PortName;
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
        public event Action<ConnectDataEvent> OnConnectDataEvent;
        /// <summary>
        /// 
        /// </summary>
        public event Action<PromptEvent> OnPromptEvent;
        /// <summary>
        /// 
        /// </summary>
        public event Action<string> OnLogCallback;

        async void ReadData()
        {
            while (_serialPort.IsOpen)
            {
                try
                {
                    while (_serialPort.BytesToRead > 0)
                    {
                        var line = await _serialPort.BaseStream.ReadToAsync(Consts.LineBreak);

                        if (string.IsNullOrWhiteSpace(line))
                        {
                            _WriteReceivedLog(line);
                            continue;
                        }

                        if (line.StartsWith("AT+") && line.EndsWith("\r")) //response for command AT+CLIP=1\r
                        {
                            _WriteReceivedLog(line);
                            continue;
                        }

                        switch (line)
                        {
                            case "OK":
                                OnCommandResult?.Invoke(true);
                                _WriteReceivedLog(line);
                                break;

                            case "ERROR":
                                OnCommandResult?.Invoke(false);
                                _WriteReceivedLog(line);
                                break;

                            case "ATA\r":
                                _WriteReceivedLog(line);
                                break;

                            case "Call Ready":
                            case "NO CARRIER":
                            case "RING":
                                OnUnknowReceived?.Invoke(line);
                                _WriteReceivedLog(line);
                                break;

                            case "CONNECT":
                                _WriteReceivedLog(line);
                                ConnectDataEvent connectDataEvent = new ConnectDataEvent(_serialPort.BaseStream);
                                OnConnectDataEvent?.Invoke(connectDataEvent);
                                if (await connectDataEvent.WaitSomeOneTakeItAsync(2000))
                                {
                                    await connectDataEvent.WaitStreamDisposeAsync();
                                }
                                break;

                            default:
                                await _ParseLine(line);
                                break;
                        }
                    }
                    await Task.Delay(100);
                }
                catch (Exception ex)
                {
#if DEBUG
                    Console.WriteLine($"{ex.GetType().FullName}: {ex.Message}, {ex.StackTrace}");
#endif
                }
            }
        }

        async Task _ParseLine(string line)
        {
            const string cme_err = "+CME ERROR:";
            const string cms_err = "+CMS ERROR:";
            if (line.StartsWith(cme_err))
            {
                string num = line.Substring(cme_err.Length).Trim();
                if (int.TryParse(num, out int n))
                {
                    string err_msg = string.Empty;
                    if (Consts.CME_Error.ContainsKey(n)) err_msg = $"{Consts.CME_Error[n]} ({n})";
                    else err_msg = n.ToString();
                    OnMeError?.Invoke(err_msg, n);
                    _WriteReceivedLog(line);
                }
                return;
            }
            else if (line.StartsWith(cms_err))
            {
                string num = line.Substring(cms_err.Length).Trim();
                if (int.TryParse(num, out int n))
                {
                    string err_msg = string.Empty;
                    if (Consts.CMS_Error.ContainsKey(n)) err_msg = $"{Consts.CMS_Error[n]} ({n})";
                    else err_msg = n.ToString();
                    OnMsError?.Invoke(err_msg, n);
                    _WriteReceivedLog(line);
                }
                return;
            }
            else
            {
                //regex +ABC: data,data2,data3
                Regex regex_responseCommand = new Regex("\\+([A-z0-9]+):([\\x01-\\x0C\\x0E-\\x7E]+)$");
                Match match = regex_responseCommand.Match(line);
                if (match.Success)
                {
                    string command = match.Groups[1].Value.Trim();
                    string args = match.Groups[2].Value.Trim();
                    _WriteReceivedLog(line);

                    Regex regex_csv_pattern = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                    if (args.StartsWith("(") && args.EndsWith(")"))//this is Test
                    {
                        string[] options_split = Regex.Split(args.Substring(1, args.Length - 2), "\\),\\(");
                        _BodyInvokeAndPrint(new GsmCommandResponse(command, options_split.Select(x => regex_csv_pattern.Split(x)).ToList()));
                        return;
                    }
                    else
                    {
                        string data = string.Empty;
                        switch (command)
                        {
                            case "CMT":
                                data = await _serialPort.BaseStream.ReadToAsync(Consts.LineBreak);
                                break;
                        }

                        string[] args_split = regex_csv_pattern.Split(args);
                        _BodyInvokeAndPrint(new GsmCommandResponse(command, args_split, data));
                        return;
                    }
                }

                if (line.Any(x => !(x == 10 || x == 13 || (x >= 32 && x <= 126))))//not CR LF or text char 32 -> 126
                {
                    //this is binary
                    _WriteReceivedLog($"Binary[{line.Length}]");
                }
                else
                {
                    OnUnknowReceived?.Invoke(line);
                    _WriteReceivedLog(line);
                }

            }
        }

        void _WriteReceivedLog(string log, string name = "")
        {
#if DEBUG
            Console.WriteLine($"{PortName} >> {log.PrintCRLFHepler()}");
#endif
            if (OnLogCallback != null)
            {
                string _log = string.IsNullOrWhiteSpace(name) ? string.Empty : $"[{name}]";
                _log += $"{PortName} >> {log.Trim()}";
                OnLogCallback?.Invoke(_log);
            }
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        public void Write(string command)
        {
            _serialPort.Write(command);
        }
        /// <summary>
        /// 
        /// </summary>
        public void Open()
        {
            _serialPort.Open();
            ClearAvalableData();

            if (_synchronizationContext is not null)
                _synchronizationContext.Send(ReadData);
            else
                ThreadPool.QueueUserWorkItem((o) => ReadData());
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearAvalableData()
        {
            byte[] buffer = new byte[1024];
            while (_serialPort.IsOpen && _serialPort.BytesToRead > 0)
            {
                _serialPort.Read(buffer, 0, buffer.Length);
            }
        }

    }
}
