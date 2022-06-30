using System.Collections.Generic;
using System.Linq;
using System;
namespace TqkLibrary.GSM
{
    public class GsmCommandResult
    {
        public bool IsSuccess { get; internal set; }
        public IEnumerable<GsmCommandResponse> CommandResponses => _CommandResponses;
        public IEnumerable<string> Datas => _Datas;



        internal List<GsmCommandResponse> _CommandResponses { get; } = new List<GsmCommandResponse>();
        internal List<string> _Datas { get; } = new List<string>();

        public GsmCommandResponse GetCommandResponse(string name)
        {
            if(string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            return CommandResponses.FirstOrDefault(x => name.Equals(x.Command));
        }
    }
}