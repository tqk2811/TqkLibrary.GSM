using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TqkLibrary.GSM.AtClient;

namespace TqkLibrary.GSM.Test
{
    [TestClass]
    public class AtClientLoopReadLine_Parse_Test
    {
        [TestMethod]
        public async Task TestAsync()
        {
            AtClientLoopReadLine atClientLoopReadLine = new AtClientLoopReadLine("abc");
            await atClientLoopReadLine._ParseLine("+CUSD: 2,\"Ban dang o MSHN02H. So may cua ban la:\r84936514135\",15");
        }
    }
}
