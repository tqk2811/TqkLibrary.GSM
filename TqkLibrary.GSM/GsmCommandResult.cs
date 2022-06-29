using System.Collections.Generic;

namespace TqkLibrary.GSM
{
    public class GsmCommandResult
    {
        public bool IsSuccess { get; internal set; }
        public IDictionary<string, GsmCommandResponse> CommandResponses => _CommandResponses;
        public IEnumerable<string> Datas => _Datas;



        internal Dictionary<string, GsmCommandResponse> _CommandResponses { get; } = new Dictionary<string, GsmCommandResponse>();
        internal List<string> _Datas { get; } = new List<string>();
    }
}