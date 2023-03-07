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
        private GsmCommandResponse(string command, string[] args, string data)
        {
            this.Command = command;
            this._Arguments.AddRange(args);
            this.Data = data;
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
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Command: {Command}, Arguments: [{string.Join(";", Arguments)}], Data: {Data}";
        }




        readonly List<string> _Arguments = new List<string>();
        readonly List<IEnumerable<string>> _Options = new List<IEnumerable<string>>();

        private static readonly Regex regex_Command = new Regex("^\\+([A-z0-9]+):([\\x01-\\x0C\\x0E-\\x7E]+)(|\\r\\n[\\x01-\\x7E]+)$", RegexOptions.Multiline);
        private static readonly Regex regex_csv_pattern = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
        public static GsmCommandResponse Parse(string str)
        {
            Match match = regex_Command.Match(str);
            if (match.Success)
            {
                string command = match.Groups[1].Value.Trim();
                string args = match.Groups[2].Value.Trim();
                if (args.StartsWith("(") && args.EndsWith(")"))
                {
                    string[] options_split = Regex.Split(args.Substring(1, args.Length - 2),"\\),\\(");
                    return new GsmCommandResponse(command, options_split.Select(x => regex_csv_pattern.Split(x)).ToList());
                }
                else
                {
                    string data = match.Groups[3].Value.Trim();
                    string[] args_split = regex_csv_pattern.Split(args);
                    return new GsmCommandResponse(command, args_split, data);
                }
            }
            return null;
        }
    }
}