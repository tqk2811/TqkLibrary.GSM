using System.Collections.Generic;

namespace GsmAtWrapper
{
    public class GsmCommandResult
    {
        public bool IsSuccess { get; internal set; }
        public IDictionary<string, GsmCommand> CommandResponses => _CommandResponses;
        public IEnumerable<string> Datas => _Datas;



        internal Dictionary<string, GsmCommand> _CommandResponses { get; } = new Dictionary<string, GsmCommand>();
        internal List<string> _Datas { get; } = new List<string>();
    }

    public class GsmCommand
    {
        internal GsmCommand(string[] args, string data)
        {
            this._Arguments.AddRange(args);
            this.Data = data;
        }
        public IEnumerable<string> Arguments => _Arguments;
        public string Data { get; internal set; }

        internal readonly List<string> _Arguments = new List<string>();
    }
}