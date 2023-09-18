using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TqkLibrary.GSM.Extensions;
using TqkLibrary.GSM.Helpers.PduPaser;
using TqkLibrary.GSM.Helpers.PduPaser.Enums;
using static TqkLibrary.GSM.Helpers.PduPaser.UserDataHeaderIndicator;

namespace TqkLibrary.GSM.Test
{
    [TestClass]
    public class TestPduCreate
    {
        [TestMethod]
        public void Test1()
        {
            string message = "Hello SMSDeliverer.com!";
            string desNumber = "0381111111";

            var pdus = Pdu.Create(desNumber, message)
                .ToList();
        }
        [TestMethod]
        public void Test2()
        {
            string message = "Rút lui , không ra thêm sản phẩm nên có thời gian chau chuốt cho bản thân, nên nhìn người có tí cơ rồi , tay này chắc một vợt lũ ruồi bệp dí , còn ruồi cha chắc một vã";
            string desNumber = "0381111111";

            var pdus = Pdu.Create(desNumber, message)
                .ToList();
        }
    }
}
