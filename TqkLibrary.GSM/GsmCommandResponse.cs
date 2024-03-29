﻿namespace TqkLibrary.GSM
{
    /// <summary>
    /// 
    /// </summary>
    public class GsmCommandResponse
    {
        internal GsmCommandResponse(string command, string[] args, string data)
        {
            Command = command;
            _Arguments.AddRange(args);
            Data = data;
        }
        internal GsmCommandResponse(string command, IEnumerable<IEnumerable<string>> options)
        {
            Command = command;
            _Options.AddRange(options);
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
    }
}