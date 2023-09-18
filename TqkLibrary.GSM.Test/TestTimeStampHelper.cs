using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TqkLibrary.GSM.Extensions;
using TqkLibrary.GSM.Helpers.PduPaser;
using TqkLibrary.GSM.Helpers.PduPaser.Enums;

namespace TqkLibrary.GSM.Test
{
    [TestClass]
    public class TestTimeStampHelper
    {
        [TestMethod]
        public void Test()
        {
            //remove mili second or lower for check Assert
            var str_time = DateTime.Now.ToString();
            var time = DateTime.Parse(str_time);

            var buffer = ServiceCentreTimeStamp.DateTimeToBuffer(time);
            var parse = ServiceCentreTimeStamp.ParseDateTime(buffer);
            Assert.IsTrue(parse.Equals(time));
        }
    }
}
