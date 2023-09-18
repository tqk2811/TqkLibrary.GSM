using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TqkLibrary.GSM.Extensions;
using TqkLibrary.GSM.PDU;
using TqkLibrary.GSM.PDU.Enums;
using static TqkLibrary.GSM.PDU.UserDataHeaderIndicator;

namespace TqkLibrary.GSM.Test.PDU
{
    [TestClass]
    public class ValidityPeriodTest
    {
        public static IEnumerable<object[]> AdditionData_PresentAndIntegerRepresented
        {
            get
            {
                return new[]
                {
                    new object[] { ValidityPeriodFormat.PresentAndIntegerRepresented, TimeSpan.Zero, TimeSpan.Zero },
                    //round 5 min
                    new object[] { ValidityPeriodFormat.PresentAndIntegerRepresented, TimeSpan.FromMinutes(54), TimeSpan.FromMinutes(55) },
                    //round 30 min, 14:12 -> 14:30
                    new object[] { ValidityPeriodFormat.PresentAndIntegerRepresented, TimeSpan.FromHours(14.2), TimeSpan.FromHours(14.5) },
                    //round day
                    new object[] { ValidityPeriodFormat.PresentAndIntegerRepresented, TimeSpan.FromDays(2.2), TimeSpan.FromDays(3) },
                    //round week
                    new object[] { ValidityPeriodFormat.PresentAndIntegerRepresented, TimeSpan.FromDays(31.6), TimeSpan.FromDays(7 * 5) },
                };
            }
        }

        [TestMethod]
        [DynamicData(nameof(AdditionData_PresentAndIntegerRepresented))]
        public void Test_PresentAndIntegerRepresented(ValidityPeriodFormat validityPeriodFormat, TimeSpan input, TimeSpan check)
        {
            string message = "Hello SMSDeliverer.com!";
            string desNumber = "0381111111";

            var pdus = Pdu.Create(desNumber, message)
                .ToList();

            foreach (var pdu in pdus)
            {
                pdu.PduHeader.ValidityPeriodFormat = validityPeriodFormat;
                pdu.ValidityPeriod.Time = input;
                Assert.AreEqual(pdu.ValidityPeriod.Time, check);
            }
        }
    }
}
