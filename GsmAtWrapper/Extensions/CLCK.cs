using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GsmAtWrapper.Extensions
{
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

    public enum CLCK_Mode
    {
        Unlock = 0,
        Lock = 1,
        QueryStatus = 2
    }
    [Flags]
    public enum CLCK_Class
    {
        Voice = 1,
        Data = 2,
        Fax = 4,
        ShortMessageService = 8,
        DataCircuitSync = 16,
        DataCircuitAsync = 32,
        DedicatedPacketAccess = 64,
        Dedicated_PAD_Access = 128
    }
    public static partial class GsmExtensions
    {
        /// <summary>
        /// 3.6.2.3.5 +CLCK - Facility Lock/ Unlock <br></br>reboot needed
        /// </summary>
        /// <returns></returns>
        public static Task<GsmCommandResult> WriteFacility(this GsmClient gsmClient,
            CLCK_FAC fac,
            CLCK_Mode mode,
            CancellationToken cancellationToken = default)
            => gsmClient.Write("CLCK", cancellationToken, fac.ToAtString(), (int)mode);

        /// <summary>
        /// 3.6.2.3.5 +CLCK - Facility Lock/ Unlock <br></br>reboot needed
        /// </summary>
        /// <returns></returns>
        public static Task<GsmCommandResult> WriteFacility(this GsmClient gsmClient,
            CLCK_FAC fac,
            CLCK_Mode mode,
            string pin,
            CancellationToken cancellationToken = default)
            => gsmClient.Write("CLCK", cancellationToken, fac.ToAtString(), (int)mode, pin.ToAtString());

        /// <summary>
        /// 3.6.2.3.5 +CLCK - Facility Lock/ Unlock <br></br>reboot needed
        /// </summary>
        /// <returns></returns>
        public static Task<GsmCommandResult> WriteFacility(this GsmClient gsmClient,
            CLCK_FAC fac,
            CLCK_Mode mode,
            string pin,
            CLCK_Class @class,
            CancellationToken cancellationToken = default)
            => gsmClient.Write("CLCK", cancellationToken, fac.ToAtString(), (int)mode, pin.ToAtString(), (int)@class);


        public static Task<bool> TestFacility(this GsmClient gsmClient, CancellationToken cancellationToken = default)
            => gsmClient.Test("CLCK", cancellationToken).GetTaskResult(x => x.IsSuccess);
    }
}
