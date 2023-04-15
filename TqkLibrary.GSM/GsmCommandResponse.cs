using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TqkLibrary.GSM
{
    /// <summary>
    /// 
    /// </summary>
    public class GsmCommandResponse
    {
        private GsmCommandResponse(string command, string[] args, string data, byte[] binary)
        {
            this.Command = command;
            this._Arguments.AddRange(args);
            this.Data = data;
            this.BinaryData = binary ?? new byte[0];
        }
        private GsmCommandResponse(string command, IEnumerable<IEnumerable<string>> options)
        {
            this.Command = command;
            this._Options.AddRange(options);
        }
        /// <summary>
        /// 
        /// </summary>
        public string Command { get; }
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<string> Arguments => _Arguments;
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<IEnumerable<string>> Options => _Options;
        /// <summary>
        /// 
        /// </summary>
        public string Data { get; }
        /// <summary>
        /// for CONNECT response
        /// </summary>
        public IEnumerable<byte> BinaryData { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Command: {Command}, Arguments: [{string.Join(";", Arguments)}], Data: {Data}";
        }




        readonly List<string> _Arguments = new List<string>();
        readonly List<IEnumerable<string>> _Options = new List<IEnumerable<string>>();

        private static readonly Regex regex_Command 
            = new Regex("^(CONNECT\r\n[\\x00-\\xFF]*?\r\n|)\\+([A-z0-9]+):([\\x01-\\x0C\\x0E-\\x7E]+)(|\\r\\n[\\x01-\\x7E]+)$", RegexOptions.Multiline);
        private static readonly Regex regex_csv_pattern 
            = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static GsmCommandResponse Parse(string str)
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
                    binary = GsmClient.GsmEncoding.GetBytes(connect_binary_str)
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