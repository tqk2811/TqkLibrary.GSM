namespace TqkLibrary.GSM.Extended
{
    /// <summary>
    /// 
    /// </summary>
    public class CommandRequestCLCK : CommandRequest
    {
        internal CommandRequestCLCK(IGsmClient gsmClient) : base(gsmClient, "CLCK")
        {

        }
        /// <summary>
        /// reboot needed
        /// </summary>
        /// <returns></returns>
        public Task<GsmCommandResult> WriteFacilityAsync(
            CLCK_FAC fac,
            CLCK_Mode mode,
            CancellationToken cancellationToken = default)
            => GsmClient.WriteAsync("CLCK", cancellationToken, fac.ToAtString(), (int)mode);

        /// <summary>
        /// reboot needed
        /// </summary>
        /// <returns></returns>
        public Task<GsmCommandResult> WriteFacilityAsync(
            CLCK_FAC fac,
            CLCK_Mode mode,
            string pin,
            CancellationToken cancellationToken = default)
            => GsmClient.WriteAsync("CLCK", cancellationToken, fac.ToAtString(), (int)mode, pin.ToAtString());

        /// <summary>
        /// reboot needed
        /// </summary>
        /// <returns></returns>
        public Task<GsmCommandResult> WriteFacilityAsync(
            CLCK_FAC fac,
            CLCK_Mode mode,
            string pin,
            CLCK_Class @class,
            CancellationToken cancellationToken = default)
            => GsmClient.WriteAsync("CLCK", cancellationToken, fac.ToAtString(), (int)mode, pin.ToAtString(), (int)@class);


        /// <summary>
        /// 
        /// </summary>
        public enum CLCK_FAC
        {
            /// <summary>
            /// SIM (PIN request) (device asks SIM password at power-up and when this lock command issued)
            /// </summary>
            SC,
            /// <summary>
            /// BAOC (Barr All Outgoing Calls)
            /// </summary>
            AO,
            /// <summary>
            /// BOIC (Barr Outgoing International Calls)
            /// </summary>
            OI,
            /// <summary>
            /// BOIC-exHC (Barr Outgoing International Calls except to Home Country)
            /// </summary>
            OX,
            /// <summary>
            /// BAIC (Barr All Incoming Calls)
            /// </summary>
            AI,
            /// <summary>
            /// BIC-Roam (Barr Incoming Calls when Roaming outside the home country)
            /// </summary>
            IR,
            /// <summary>
            /// All Barring services (applicable only for &lt;mode&gt;=0)
            /// </summary>
            AB,
            /// <summary>
            /// All outGoing barring services (applicable only for &lt;mode&gt;=0)
            /// </summary>
            AG,
            /// <summary>
            /// All inComing barring services (applicable only for &lt;mode&gt;=0) 
            /// </summary>
            AC,
            /// <summary>
            /// SIM fixed dialling memory feature (if PIN2 authentication has not been done during the current session, PIN2 is required as &lt;passwd&gt;)
            /// </summary>
            FD,
            /// <summary>
            /// network Personalisation
            /// </summary>
            PN,
            /// <summary>
            /// network subset Personalisation
            /// </summary>
            PU
        }
        /// <summary>
        /// 
        /// </summary>
        public enum CLCK_Mode
        {
            /// <summary>
            /// 
            /// </summary>
            Unlock = 0,
            /// <summary>
            /// 
            /// </summary>
            Lock = 1,
            /// <summary>
            /// 
            /// </summary>
            QueryStatus = 2
        }
        /// <summary>
        /// 
        /// </summary>
        [Flags]
        public enum CLCK_Class
        {
            /// <summary>
            /// 
            /// </summary>
            Voice = 1,
            /// <summary>
            /// 
            /// </summary>
            Data = 2,
            /// <summary>
            /// 
            /// </summary>
            Fax = 4,
            /// <summary>
            /// 
            /// </summary>
            ShortMessageService = 8,
            /// <summary>
            /// 
            /// </summary>
            DataCircuitSync = 16,
            /// <summary>
            /// 
            /// </summary>
            DataCircuitAsync = 32,
            /// <summary>
            /// 
            /// </summary>
            DedicatedPacketAccess = 64,
            /// <summary>
            /// 
            /// </summary>
            Dedicated_PAD_Access = 128
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public static class CommandRequestCLCKExtension
    {
        /// <summary>
        /// Facility Lock/ Unlock
        /// </summary>
        /// <param name="gsmClient"></param>
        /// <returns></returns>
        public static CommandRequestCLCK CLCK(this IGsmClient gsmClient) => new CommandRequestCLCK(gsmClient);
    }
}
