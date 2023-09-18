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
    public class ValidityPeriodTest
    {
        [TestMethod]
        public void Test1()
        {
            string message = "Hello SMSDeliverer.com!";
            string desNumber = "0381111111";

            var pdus = PDU.Create(desNumber, message)
                .ToList();

            foreach (var pdu in pdus)
            {
                pdu.PduHeader.ValidityPeriodFormat = ValidityPeriodFormat.PresentAndIntegerRepresented;

                pdu.ValidityPeriod.Time = TimeSpan.Zero;
                Assert.AreEqual(pdu.ValidityPeriod.Time, TimeSpan.Zero);

                //round 5 min
                pdu.ValidityPeriod.Time = TimeSpan.FromMinutes(54);
                Assert.AreEqual(pdu.ValidityPeriod.Time, TimeSpan.FromMinutes(55));

                //round 30 min
                pdu.ValidityPeriod.Time = TimeSpan.FromHours(14.2);//14:12
                Assert.AreEqual(pdu.ValidityPeriod.Time, TimeSpan.FromHours(14.5));//14:30

                //round day
                pdu.ValidityPeriod.Time = TimeSpan.FromDays(2.2);
                Assert.AreEqual(pdu.ValidityPeriod.Time, TimeSpan.FromDays(3));

                //round week
                pdu.ValidityPeriod.Time = TimeSpan.FromDays(31.6);//4 week + 4 day
                Assert.AreEqual(pdu.ValidityPeriod.Time, TimeSpan.FromDays(7 * 5));//5 week



                pdu.PduHeader.ValidityPeriodFormat = ValidityPeriodFormat.PresentAndSemiOctetRepresented;

                pdu.ValidityPeriod.Time = TimeSpan.Zero;
                Assert.AreEqual(pdu.ValidityPeriod.Time, TimeSpan.Zero);

                //round 5 min
                pdu.ValidityPeriod.Time = TimeSpan.FromMinutes(54);
                Assert.AreEqual(pdu.ValidityPeriod.Time, TimeSpan.FromMinutes(55));

                //round 30 min
                pdu.ValidityPeriod.Time = TimeSpan.FromHours(14.2);//14:12
                Assert.AreEqual(pdu.ValidityPeriod.Time, TimeSpan.FromHours(14.5));//14:30

                //round day
                pdu.ValidityPeriod.Time = TimeSpan.FromDays(2.2);
                Assert.AreEqual(pdu.ValidityPeriod.Time, TimeSpan.FromDays(3));

                //round week
                pdu.ValidityPeriod.Time = TimeSpan.FromDays(31.6);//4 week + 4 day
                Assert.AreEqual(pdu.ValidityPeriod.Time, TimeSpan.FromDays(7 * 5));//5 week
            }
        }
    }
}
