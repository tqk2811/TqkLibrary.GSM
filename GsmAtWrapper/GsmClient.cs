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


        public event Action<bool> OnResponseOK;
        public event Action OnCallReady;
        public event Action<string, string[]> OnCommandReceived;
        public event Action<string> OnUnknowReceived;
        public event Action<string> OnCommandError;

        private static readonly Regex regex = new Regex(@"(?<=^\r?\n|[\x01-\x7E]\r?\n\r?\n)([\x01-\x7E]*?)(?=\r?\n\r?\n[\x01-\x7E]|\r?\n$)");
        private static readonly Regex regex_Command = new Regex("^\\+([A-z0-9]+):([\\x20-\\x7E\\r\\n]+)$", RegexOptions.Multiline);
        private static readonly Regex regex_csv_partent = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

        private string temp = string.Empty;

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] buffer = new byte[serialPort.ReadBufferSize];
            int byteRead = serialPort.Read(buffer, 0, buffer.Length);
            string received = temp + Encoding.UTF8.GetString(buffer, 0, byteRead);
            Console.WriteLine($">>{received.TrimStart()}");
            if (received.EndsWith("\r\n"))
            {
                temp = string.Empty;
                var matches = regex.Matches(received);
                foreach (Match match in matches)
                {
                    var matchString = match.Groups[1].Value;

                    if (matchString.StartsWith("OK"))
                    {
                        OnResponseOK?.Invoke(true);
                        continue;
                    }

                    if (matchString.StartsWith("Call Ready"))
                    {
                        OnCallReady?.Invoke();
                        continue;
                    }

                    if (matchString.StartsWith("+"))
                    {
                        Match match_cmd = regex_Command.Match(matchString);
                        if (match_cmd.Success)
                        {
                            string command = match_cmd.Groups[1].Value;
                            string values = match_cmd.Groups[2].Value.Trim();
                            string[] values_split = regex_csv_partent.Split(values);
                            
                            OnCommandReceived?.Invoke(command, values_split);
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

        public void TestSend()
        {
            while (true)
            {
                string command = Console.ReadLine();
                serialPort.Write($"AT{command}\r\n");
            }
        }
        // ?
        public async Task<bool> Test(string command, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(command)) throw new ArgumentNullException(nameof(command));
            using (await asyncLockSend.LockAsync(cancellationToken).ConfigureAwait(false))
            {
                TaskCompletionSource<bool> tcs_ok = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                Action<bool> action_ok = (r) => tcs_ok.TrySetResult(r);
                using var register = cancellationToken.Register(() => tcs_ok.TrySetCanceled());
                try
                {
                    OnResponseOK += action_ok;

                    Console.WriteLine($"<<AT{command}=?");
                    serialPort.Write($"AT{command}=?\r\n");

                    await tcs_ok.Task.ConfigureAwait(false);
                    return tcs_ok.Task.Result;
                }
                finally
                {
                    OnResponseOK -= action_ok;
                }
            }
        }

        // + ?
        public async Task<string[]> Read(string command, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(command)) throw new ArgumentNullException(nameof(command));
            using (await asyncLockSend.LockAsync(cancellationToken).ConfigureAwait(false))
            {
                TaskCompletionSource<bool> tcs_ok = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                Action<bool> action_ok = (r) => tcs_ok.TrySetResult(r);

                TaskCompletionSource<string[]> tcs_command = new TaskCompletionSource<string[]>(TaskCreationOptions.RunContinuationsAsynchronously);
                Action<string, string[]> action_command = (c, vals) =>
                {
                    if (command.Equals(c))
                        tcs_command.TrySetResult(vals);
                };
                using var register = cancellationToken.Register(() => tcs_command.TrySetCanceled());
                try
                {
                    OnCommandReceived += action_command;
                    OnResponseOK += action_ok;

                    Console.WriteLine($"<<AT+{command}?");
                    serialPort.Write($"AT+{command}?\r\n");

                    await tcs_ok.Task.ConfigureAwait(false);
                    tcs_command.TrySetResult(new string[] { });
                    return tcs_command.Task.Result;
                }
                finally
                {
                    OnResponseOK -= action_ok;
                    OnCommandReceived -= action_command;
                }
            }
        }


        public Task<WriteResult> WriteGetResult(string command, string[] values, CancellationToken cancellationToken = default)
            => WriteGetResult(command, string.Join(",", values), cancellationToken);
        public Task<WriteResult> WriteGetResult(string command, object[] values, CancellationToken cancellationToken = default)
            => WriteGetResult(command, string.Join(",", values), cancellationToken);
        public Task<WriteResult> WriteGetResult(string command, CancellationToken cancellationToken = default, params object[] values)
            => WriteGetResult(command, string.Join(",", values), cancellationToken);
        public async Task<WriteResult> WriteGetResult(string command, string value, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(command)) throw new ArgumentNullException(nameof(command));
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(nameof(value));
            using (await asyncLockSend.LockAsync(cancellationToken).ConfigureAwait(false))
            {
                WriteResult writeResult = new WriteResult();

                TaskCompletionSource<bool> tcs_ok = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                Action<bool> action_ok = (r) => tcs_ok.TrySetResult(r);

                Action<string, string[]> action_command = (c, vals) =>
                {
                    if (command.Equals(c))
                        writeResult._Datas.Add(vals);
                };

                using var register = cancellationToken.Register(() => tcs_ok.TrySetCanceled());
                try
                {
                    OnResponseOK += action_ok;
                    OnCommandReceived += action_command;

                    Console.WriteLine($"<<AT+{command}={value}");
                    serialPort.Write($"AT+{command}={value}\r\n");

                    await Task.WhenAll(tcs_ok.Task).ConfigureAwait(false);

                    writeResult.IsSuccess = tcs_ok.Task.Result;
                    return writeResult;
                }
                finally
                {
                    OnResponseOK -= action_ok;
                    OnCommandReceived -= action_command;
                }
            }
        }



        public Task<bool> Write(string command, CancellationToken cancellationToken = default, params object[] values)
            => Write(command, string.Join(",", values), cancellationToken);
        public Task<bool> Write(string command, string[] values, CancellationToken cancellationToken = default)
            => Write(command, string.Join(",", values), cancellationToken);
        // + = 
        public async Task<bool> Write(string command, string value, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(command)) throw new ArgumentNullException(nameof(command));
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(nameof(value));
            using (await asyncLockSend.LockAsync(cancellationToken).ConfigureAwait(false))
            {
                TaskCompletionSource<bool> tcs_ok = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                Action<bool> action_ok = (r) => tcs_ok.TrySetResult(r);

                using var register = cancellationToken.Register(() => tcs_ok.TrySetCanceled());
                try
                {
                    OnResponseOK += action_ok;

                    Console.WriteLine($"<<AT+{command}={value}");
                    serialPort.Write($"AT+{command}={value}\r\n");

                    await tcs_ok.Task.ConfigureAwait(false);
                    return tcs_ok.Task.Result;
                }
                finally
                {
                    OnResponseOK -= action_ok;
                }
            }
        }

        // +
        public async Task<string[]> Execute(string command, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(command)) throw new ArgumentNullException(nameof(command));
            using (await asyncLockSend.LockAsync(cancellationToken).ConfigureAwait(false))
            {

                TaskCompletionSource<bool> tcs_ok = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                Action<bool> action_ok = (r) => tcs_ok.TrySetResult(r);

                TaskCompletionSource<string[]> tcs_command = new TaskCompletionSource<string[]>(TaskCreationOptions.RunContinuationsAsynchronously);
                Action<string, string[]> action_command = (c, vals) =>
                {
                    if (command.Equals(c))
                        tcs_command.TrySetResult(vals);
                };
                Action<string> action_unknow = (c) =>
                {
                    tcs_command.TrySetResult(new string[] { c });
                };
                using var register = cancellationToken.Register(() => tcs_command.TrySetCanceled());
                try
                {
                    OnResponseOK += action_ok;
                    OnCommandReceived += action_command;
                    OnUnknowReceived += action_unknow;
                    Console.WriteLine($"<<AT+{command}");
                    serialPort.Write($"AT+{command}\r\n");

                    await tcs_ok.Task.ConfigureAwait(false);
                    tcs_command.TrySetResult(new string[] { });
                    return tcs_command.Task.Result;
                }
                finally
                {
                    OnResponseOK -= action_ok;
                    OnCommandReceived -= action_command;
                    OnUnknowReceived -= action_unknow;
                }
            }
        }
    }
}