using System.Collections.Generic;

namespace GsmAtWrapper
{
    public class GsmCommandResult
    {
        public bool IsSuccess { get; internal set; }
        public IDictionary<string, IEnumerable<string>> CommandResponses => _CommandResponses;
        public IEnumerable<string> Datas => _Datas;



        internal Dictionary<string, IEnumerable<string>> _CommandResponses { get; } = new Dictionary<string, IEnumerable<string>>();
        internal List<string> _Datas { get; } = new List<string>();
    }
}