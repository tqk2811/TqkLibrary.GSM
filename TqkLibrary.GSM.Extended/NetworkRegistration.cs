namespace TqkLibrary.GSM.Extended
{
    /// <summary>
    /// Shared base for +CREG, +CGREG and +CEREG. 3GPP TS 27.007 gives the three
    /// commands the same parameter layout, so the parsing lives here once.
    /// </summary>
    public abstract class CommandRequestNetworkRegistration : CommandRequest
    {
        internal CommandRequestNetworkRegistration(IGsmClient gsmClient, string command)
            : base(gsmClient, command)
        {

        }

        /// <summary>
        /// Enable or disable the unsolicited registration result code.
        /// </summary>
        /// <param name="urcMode"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<bool> WriteAsync(RegistrationUrcMode urcMode, CancellationToken cancellationToken = default)
            => GsmClient.WriteAsync(Command, ((int)urcMode).ToString(), cancellationToken).GetTaskResult(x => x.IsSuccess);

        /// <summary>
        /// Read the current registration state, or null when the modem gives no parsable answer.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public new async Task<RegistrationState> ReadAsync(CancellationToken cancellationToken = default)
        {
            var result = await base.ReadAsync(cancellationToken).ConfigureAwait(false);
            return RegistrationState.Parse(result.GetCommandResponse(Command));
        }

        /// <summary>
        /// &lt;n&gt; - controls the unsolicited result code.
        /// </summary>
        public enum RegistrationUrcMode
        {
            /// <summary>
            /// disable the unsolicited result code
            /// </summary>
            Disable = 0,
            /// <summary>
            /// enable the unsolicited result code +CREG: &lt;stat&gt;
            /// </summary>
            Enable = 1,
            /// <summary>
            /// enable the unsolicited result code with location information
            /// </summary>
            EnableWithLocation = 2,
        }

        /// <summary>
        /// &lt;stat&gt; - the registration state reported by the modem.
        /// </summary>
        public enum RegistrationStat
        {
            /// <summary>
            /// not registered, the modem is not currently searching for an operator
            /// </summary>
            NotRegisteredNotSearching = 0,
            /// <summary>
            /// registered, home network
            /// </summary>
            RegisteredHomeNetwork = 1,
            /// <summary>
            /// not registered, but the modem is searching for an operator
            /// </summary>
            NotRegisteredSearching = 2,
            /// <summary>
            /// registration denied
            /// </summary>
            RegistrationDenied = 3,
            /// <summary>
            /// unknown
            /// </summary>
            Unknown = 4,
            /// <summary>
            /// registered, roaming
            /// </summary>
            RegisteredRoaming = 5,
        }

        /// <summary>
        /// A parsed +CREG / +CGREG / +CEREG payload.
        /// </summary>
        public class RegistrationState
        {
            /// <summary>
            /// Only carried by a read reply; an unsolicited result code does not repeat it.
            /// </summary>
            public RegistrationUrcMode? UrcMode { get; internal set; }
            /// <summary>
            /// 
            /// </summary>
            public RegistrationStat Stat { get; internal set; }
            /// <summary>
            /// 
            /// </summary>
            public string LocationAreaCode { get; internal set; }
            /// <summary>
            /// 
            /// </summary>
            public string CellId { get; internal set; }
            /// <summary>
            /// &lt;AcT&gt; - access technology, reported only by some modems.
            /// </summary>
            public int? AccessTechnology { get; internal set; }
            /// <summary>
            /// True when the modem is attached to a network, whether at home or roaming.
            /// </summary>
            public bool IsRegistered
                => Stat == RegistrationStat.RegisteredHomeNetwork || Stat == RegistrationStat.RegisteredRoaming;

            /// <summary>
            /// Parses both shapes the modem can send.
            /// <para>A read reply carries &lt;n&gt; first: "+CREG: 1,5".</para>
            /// <para>An unsolicited result code starts at &lt;stat&gt;: "+CREG: 2" or "+CREG: 1,"00A2","1B0D"".</para>
            /// They are told apart by the second field: a read reply has a bare &lt;stat&gt; digit there,
            /// while the unsolicited form has a quoted location area code.
            /// </summary>
            /// <param name="response"></param>
            /// <returns>null when <paramref name="response"/> is null or carries no usable state</returns>
            public static RegistrationState Parse(GsmCommandResponse response)
            {
                string[] arguments = response?.Arguments.ToArray();
                if (arguments == null || arguments.Length == 0) return null;

                bool isReadReply = arguments.Length >= 2 &&
                    int.TryParse(arguments[1], out int second) &&
                    second >= 0 &&
                    second <= 5;
                int statIndex = isReadReply ? 1 : 0;

                if (!int.TryParse(arguments[statIndex], out int stat)) return null;

                RegistrationState state = new RegistrationState();
                state.Stat = (RegistrationStat)stat;
                if (isReadReply && int.TryParse(arguments[0], out int urcMode))
                    state.UrcMode = (RegistrationUrcMode)urcMode;

                string[] rest = arguments.Skip(statIndex + 1).ToArray();
                if (rest.Length >= 2)
                {
                    state.LocationAreaCode = rest[0]?.Trim('"');
                    state.CellId = rest[1]?.Trim('"');
                }
                if (rest.Length >= 3 && int.TryParse(rest[2], out int accessTechnology))
                    state.AccessTechnology = accessTechnology;

                return state;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return $"Stat: {Stat}, UrcMode: {UrcMode}, Lac: {LocationAreaCode}, CellId: {CellId}, AcT: {AccessTechnology}";
            }
        }
    }
}
