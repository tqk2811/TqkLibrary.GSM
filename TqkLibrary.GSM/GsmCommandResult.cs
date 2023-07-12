using System.Collections.Generic;
using System.Linq;
using System;
namespace TqkLibrary.GSM
{
    /// <summary>
    /// 
    /// </summary>
    public class GsmCommandResult
    {
        /// <summary>
        /// 
        /// </summary>
        public bool IsSuccess { get; internal set; }
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<GsmCommandResponse> CommandResponses => _CommandResponses;
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<string> Datas => _Datas;



        internal List<GsmCommandResponse> _CommandResponses { get; } = new List<GsmCommandResponse>();
        internal List<string> _Datas { get; } = new List<string>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public GsmCommandResponse GetCommandResponse(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            return CommandResponses.FirstOrDefault(x => name.Equals(x.Command));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public IEnumerable<GsmCommandResponse> GetCommandResponses(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            return CommandResponses.Where(x => name.Equals(x.Command));
        }
    }
}