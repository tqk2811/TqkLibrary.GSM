using System.Collections.Generic;

namespace GsmAtWrapper
{
    public class WriteResult
    {
        public bool IsSuccess { get; set; }
        internal List<IEnumerable<string>> _Datas { get; } = new List<IEnumerable<string>>();
        public IEnumerable<IEnumerable<string>> Datas => _Datas;
    }
}