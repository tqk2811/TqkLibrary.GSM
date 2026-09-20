using System.Threading;
using System.Threading.Tasks;
using TqkLibrary.GSM.AtClient;
using TqkLibrary.GSM.Extended.Advances;

namespace TqkLibrary.GSM.Test
{
    [TestClass]
    public class SimEventUtilsRegistrationTest
    {
        sealed class Harness : IDisposable
        {
            public AtClientLoopReadLine AtClient { get; }
            public GsmClient GsmClient { get; }
            public SimEventUtils SimEventUtils { get; }
            public int Connected;
            public int Disconnected;

            public Harness()
            {
                AtClient = new AtClientLoopReadLine("abc");
                GsmClient = new GsmClient(AtClient);
                SimEventUtils = GsmClient.RegisterSimEventUtils();
                SimEventUtils.OnProviderConnected += () => Interlocked.Increment(ref Connected);
                SimEventUtils.OnProviderDisconnected += () => Interlocked.Increment(ref Disconnected);
            }

            /// <summary>
            /// The events hop through the thread pool, so give them a moment to land.
            /// </summary>
            public async Task FeedAsync(params string[] lines)
            {
                foreach (string line in lines) await AtClient._ParseLine(line);
                await Task.Delay(300);
            }

            public void Dispose()
            {
                SimEventUtils.Dispose();
                GsmClient.Dispose();
            }
        }

        /// <summary>
        /// A 3G-only SIM reports +CREG: 1,5 and +CEREG: 1,0 at the same time. The LTE indicator
        /// saying not-registered must not cancel out the circuit switched one that says registered.
        /// </summary>
        [TestMethod]
        public async Task OneIndicatorRegistered_IsEnough()
        {
            using Harness harness = new Harness();
            await harness.FeedAsync("+CREG: 1,5", "+CEREG: 1,0");
            Assert.AreEqual(1, harness.Connected);
            Assert.AreEqual(0, harness.Disconnected);
        }

        [TestMethod]
        public async Task RepeatedUrc_DoesNotRefire()
        {
            using Harness harness = new Harness();
            await harness.FeedAsync("+CREG: 1,5", "+CREG: 5,\"00A2\",\"1B0D\"", "+CREG: 1,5");
            Assert.AreEqual(1, harness.Connected);
            Assert.AreEqual(0, harness.Disconnected);
        }

        [TestMethod]
        public async Task LosingTheOnlyIndicator_Disconnects()
        {
            using Harness harness = new Harness();
            await harness.FeedAsync("+CREG: 1,5", "+CREG: 1,2");
            Assert.AreEqual(1, harness.Connected);
            Assert.AreEqual(1, harness.Disconnected);
        }

        /// <summary>
        /// Searching for an operator is the state the field board sits in, and it must never be
        /// reported as connected.
        /// </summary>
        [TestMethod]
        public async Task SearchingOnly_NeverConnects()
        {
            using Harness harness = new Harness();
            await harness.FeedAsync("+CREG: 2", "+CGREG: 2", "+CEREG: 2");
            Assert.AreEqual(0, harness.Connected);
            Assert.AreEqual(0, harness.Disconnected);
        }

        /// <summary>
        /// SIMCom modems announce readiness with "Call Ready" instead of a registration URC.
        /// </summary>
        [TestMethod]
        public async Task CallReady_StillConnects()
        {
            using Harness harness = new Harness();
            await harness.FeedAsync("Call Ready");
            Assert.AreEqual(1, harness.Connected);
        }

        [TestMethod]
        public async Task ModuleReady_FiresOnRdy()
        {
            using Harness harness = new Harness();
            int ready = 0;
            harness.SimEventUtils.OnModuleReady += () => Interlocked.Increment(ref ready);
            await harness.FeedAsync("RDY");
            Assert.AreEqual(1, ready);
            Assert.AreEqual(0, harness.Connected);
        }
    }
}
