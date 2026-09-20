using System.Threading.Tasks;
using TqkLibrary.GSM.AtClient;
using TqkLibrary.GSM.Extended;
using static TqkLibrary.GSM.Extended.CommandRequestNetworkRegistration;

namespace TqkLibrary.GSM.Test
{
    [TestClass]
    public class NetworkRegistrationParseTest
    {
        /// <summary>
        /// Feeds a raw line through the real parser so the test covers the csv split too,
        /// not just RegistrationState.Parse in isolation.
        /// </summary>
        static async Task<RegistrationState?> ParseAsync(string line)
        {
            AtClientLoopReadLine atClient = new AtClientLoopReadLine("abc");
            GsmCommandResponse? captured = null;
            atClient.OnCommandResponse += (response) => captured = response;
            await atClient._ParseLine(line);
            Assert.IsNotNull(captured, $"no command response parsed from '{line}'");
            return RegistrationState.Parse(captured);
        }

        [TestMethod]
        public async Task ReadReply_CarriesUrcModeAndStat()
        {
            RegistrationState? state = await ParseAsync("+CREG: 1,5");
            Assert.IsNotNull(state);
            Assert.AreEqual(RegistrationUrcMode.Enable, state!.UrcMode);
            Assert.AreEqual(RegistrationStat.RegisteredRoaming, state.Stat);
            Assert.IsTrue(state.IsRegistered);
        }

        [TestMethod]
        public async Task ReadReply_NotRegisteredIsNotMistakenForRegistered()
        {
            RegistrationState? state = await ParseAsync("+CEREG: 1,0");
            Assert.IsNotNull(state);
            Assert.AreEqual(RegistrationStat.NotRegisteredNotSearching, state!.Stat);
            Assert.IsFalse(state.IsRegistered);
        }

        [TestMethod]
        public async Task ReadReply_WithLocationAndAccessTechnology()
        {
            RegistrationState? state = await ParseAsync("+CREG: 2,1,\"00A2\",\"1B0D\",7");
            Assert.IsNotNull(state);
            Assert.AreEqual(RegistrationUrcMode.EnableWithLocation, state!.UrcMode);
            Assert.AreEqual(RegistrationStat.RegisteredHomeNetwork, state.Stat);
            Assert.AreEqual("00A2", state.LocationAreaCode);
            Assert.AreEqual("1B0D", state.CellId);
            Assert.AreEqual(7, state.AccessTechnology);
            Assert.IsTrue(state.IsRegistered);
        }

        [TestMethod]
        public async Task Urc_BareStat()
        {
            RegistrationState? state = await ParseAsync("+CREG: 2");
            Assert.IsNotNull(state);
            Assert.IsNull(state!.UrcMode);
            Assert.AreEqual(RegistrationStat.NotRegisteredSearching, state.Stat);
            Assert.IsFalse(state.IsRegistered);
        }

        /// <summary>
        /// The tricky one: an unsolicited code starts at &lt;stat&gt;, so the second field is a
        /// quoted location area code and must not be read as &lt;stat&gt;.
        /// </summary>
        [TestMethod]
        public async Task Urc_WithLocation_SecondFieldIsNotStat()
        {
            RegistrationState? state = await ParseAsync("+CREG: 1,\"00A2\",\"1B0D\"");
            Assert.IsNotNull(state);
            Assert.IsNull(state!.UrcMode);
            Assert.AreEqual(RegistrationStat.RegisteredHomeNetwork, state.Stat);
            Assert.AreEqual("00A2", state.LocationAreaCode);
            Assert.AreEqual("1B0D", state.CellId);
            Assert.IsTrue(state.IsRegistered);
        }

        [TestMethod]
        public async Task Urc_WithLocationAndAccessTechnology()
        {
            RegistrationState? state = await ParseAsync("+CREG: 5,\"00A2\",\"1B0D\",2");
            Assert.IsNotNull(state);
            Assert.IsNull(state!.UrcMode);
            Assert.AreEqual(RegistrationStat.RegisteredRoaming, state.Stat);
            Assert.AreEqual(2, state.AccessTechnology);
            Assert.IsTrue(state.IsRegistered);
        }

        [TestMethod]
        public void Parse_NullResponse_ReturnsNull()
        {
            Assert.IsNull(RegistrationState.Parse(null));
        }
    }
}
