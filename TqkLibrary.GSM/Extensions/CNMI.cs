using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.GSM.Extensions
{
    public enum CNMI_Mode
    {
        /// <summary>
        /// Buffer unsolicited result codes in the TA. If TA result code buffer is full, indications can be buffered in some other place or the oldest indications may be discarded and replaced with the new received indications.
        /// </summary>
        Class0 = 0,
        /// <summary>
        /// Discard indication and reject new received message unsolicited result codes when TA-TE link is reserved, otherwise forward them directly to the TE. 
        /// </summary>
        Class1 = 1,
        /// <summary>
        /// Buffer unsolicited result codes in the TA in case the DTE is busy and flush them to the TE after reservation. Otherwise forward them directly to the TE
        /// </summary>
        Class2 = 2,
        /// <summary>
        /// if <mt> is set to 1 an indication via 100 ms break is issued when a SMS is received while the module is in GPRS online mode. It enables the hardware ring line for 1 s. too.
        /// </summary>
        Class3 = 3,
    }
    public enum CNMI_MT
    {
        /// <summary>
        /// No. SMS-DELIVER indications are routed to the TE. 
        /// </summary>
        No = 0,
        /// <summary>
        /// If SMS-DELIVER is stored into ME/TA, indication of the memory location is routed to the TE using the following unsolicited result code:<br>
        /// </br>+CMTI: &lt;memr&gt;,&lt;index&gt;<br>
        /// </br>where:<br>
        /// </br> &lt;memr&gt; - memory storage where the new message is stored<br>
        /// </br>"SM"<br>
        /// </br>"ME"<br>
        /// </br>&lt;index&gt; - location on the memory where SM is stored.
        /// </summary>
        Class1 = 1,
        /// <summary>
        /// SMS-DELIVERs (except class 2 messages and messages in the message waiting indication group) are routed directly to the TE using the following unsolicited result code: <br></br><br>
        /// </br>(PDU Mode)<br></br>
        /// +CMT: &lt;alpha&gt;,&lt;length&gt;&lt;CR&gt;&lt;LF&gt;&lt;pdu&gt;<br>
        /// </br>where:<br>
        /// </br>&lt;alpha&gt; - alphanumeric representation of originator/destination<br>
        /// </br>number corresponding to the entry found in MT phonebook<br>
        /// </br>&lt;length&gt; - PDU length<br>
        /// </br>&lt;pdu&gt; - PDU message<br>
        /// </br><br></br>
        /// (TEXT Mode)<br></br>
        /// +CMT:&lt; oa&gt;,&lt; alpha&gt;,&lt; scts&gt; [,&lt;tooa&gt;,&lt;fo&gt;,&lt;pid&gt;,&lt;dcs&gt;&lt;sca&gt;,&lt;tosca&gt;,&lt;length&gt;]&lt; CR&gt;&lt; LF&gt;&lt; data&gt; (the information written in italics will be present depending on +CSDH last setting)<br></br>
        /// where:<br></br>
        /// &lt;oa&gt; - originator address number<br></br>
        /// &lt;alpha&gt; - alphanumeric representation of &lt; oa&gt; or &lt; da&gt;; used character set should be the one selected with either command +CSCS or @CSCS.<br></br>
        /// &lt;scts&gt; - arrival time of the message to the SC<br></br>
        /// &lt;tooa&gt;, &lt; tosca&gt; - type of number &lt; oa&gt; or &lt; sca&gt;:<br></br>
        /// 129 - number in national format<br></br>
        /// 145 - number in international format (contains the &quot;+&quot;)<br></br>
        /// &lt;fo&gt; - first octet of GSM 03.40<br></br>
        /// &lt;pid&gt; - Protocol Identifier<br></br>
        /// &lt;dcs&gt; - Data Coding Scheme<br></br>
        /// &lt;sca&gt; - Service Centre number<br></br>
        /// &lt;length&gt; - text length<br></br>
        /// &lt;data&gt; - TP-User-Data<br>
        /// </br><br></br>
        /// Class 2 messages and messages in the message waiting indication group (stored message) result in indication as defined in &lt;mt&gt;=1. 
        /// </summary>
        SmsDeliver = 2,

        /// <summary>
        /// Class 3 SMS-DELIVERs are routed directly to TE using unsolicited result codes defined in &lt;mt&gt;=2. Messages of other data coding schemes result in indication as defined in &lt;mt&gt;=1.
        /// </summary>
        Class3 = 3,
    }
    public enum CNMI_BM
    {
        /// <summary>
        /// Cell Broadcast Messages are not sent to the DTE
        /// </summary>
        Class0 = 0,
        /// <summary>
        /// New Cell Broadcast Messages are sent to the DTE
        /// </summary>
        Class2 = 2
    }
    public enum CNMI_DS
    {
        /// <summary>
        /// status report receiving is not reported to the DTE
        /// </summary>
        Class0 = 0,
        /// <summary>
        /// the status report is sent to the DTE
        /// </summary>
        Class1 = 1,
        /// <summary>
        /// if a status report is stored
        /// </summary>
        Class2 = 2
    }
    /// <summary>
    /// buffered result codes handling method
    /// </summary>
    public enum CNMI_BFR
    {
        /// <summary>
        /// TA buffer of unsolicited result codes defined within this command is flushed to the TE when &lt;mode&gt;=1..3 is entered (OK response shall be given before flushing the codes)
        /// </summary>
        Class0 = 0,
        /// <summary>
        /// TA buffer of unsolicited result codes defined within this command is cleared when &lt;mode&gt;=1..3 is entered.
        /// </summary>
        Class1 = 1,
    }
    public static partial class GsmExtensions
    {
        /// <summary>
        /// 3.5.3.3.1 +CNMI - New Message Indications To Terminal Equipment
        /// </summary>
        /// <returns></returns>
        public static Task<bool> WriteNewMessageIndicationsToTerminalEquipment(this GsmClient gsmClient,
            CNMI_Mode mode,
            CancellationToken cancellationToken = default)
            => gsmClient.Write("CNMI", cancellationToken, (int)mode)
            .GetTaskResult(x => x.IsSuccess);

        /// <summary>
        /// 3.5.3.3.1 +CNMI - New Message Indications To Terminal Equipment
        /// </summary>
        /// <returns></returns>
        public static Task<bool> WriteNewMessageIndicationsToTerminalEquipment(this GsmClient gsmClient,
            CNMI_Mode mode,
            CNMI_MT mt,
            CancellationToken cancellationToken = default)
            => gsmClient.Write("CNMI", cancellationToken, (int)mode, (int)mt)
            .GetTaskResult(x => x.IsSuccess);

        /// <summary>
        /// 3.5.3.3.1 +CNMI - New Message Indications To Terminal Equipment
        /// </summary>
        /// <returns></returns>
        public static Task<bool> WriteNewMessageIndicationsToTerminalEquipment(this GsmClient gsmClient,
            CNMI_Mode mode,
            CNMI_MT mt,
            CNMI_BM bm,
            CancellationToken cancellationToken = default)
            => gsmClient.Write("CNMI", cancellationToken, (int)mode, (int)mt, (int)bm)
            .GetTaskResult(x => x.IsSuccess);

        /// <summary>
        /// 3.5.3.3.1 +CNMI - New Message Indications To Terminal Equipment
        /// </summary>
        /// <returns></returns>
        public static Task<bool> WriteNewMessageIndicationsToTerminalEquipment(this GsmClient gsmClient,
            CNMI_Mode mode,
            CNMI_MT mt,
            CNMI_BM bm,
            CNMI_DS ds,
            CancellationToken cancellationToken = default)
            => gsmClient.Write("CNMI", cancellationToken, (int)mode, (int)mt, (int)bm, (int)ds)
            .GetTaskResult(x => x.IsSuccess);

        /// <summary>
        /// 3.5.3.3.1 +CNMI - New Message Indications To Terminal Equipment
        /// </summary>
        /// <returns></returns>
        public static Task<bool> WriteNewMessageIndicationsToTerminalEquipment(this GsmClient gsmClient,
            CNMI_Mode mode,
            CNMI_MT mt,
            CNMI_BM bm,
            CNMI_DS ds,
            CNMI_BFR bfr,
            CancellationToken cancellationToken = default)
            => gsmClient.Write("CNMI", cancellationToken, (int)mode, (int)mt, (int)bm, (int)ds, (int)bfr)
            .GetTaskResult(x => x.IsSuccess);

        public static Task<GsmCommandResult> TestWriteNewMessageIndicationsToTerminalEquipment(
            this GsmClient gsmClient,
            CancellationToken cancellationToken = default)
            => gsmClient.Test("CNMI", cancellationToken);

        public static async Task<CNMI_ReadResult> ReadWriteNewMessageIndicationsToTerminalEquipment(this GsmClient gsmClient,
            CancellationToken cancellationToken = default)
        {
            var result = await gsmClient.Read("CNMI", cancellationToken).ConfigureAwait(false);
            if (result.IsSuccess && result.CommandResponses.ContainsKey("CNMI") && result.CommandResponses["CNMI"].Arguments.Count() == 5)
            {
                List<int?> nums = result.CommandResponses["CNMI"].Arguments.Select(x =>
                {
                    int? result = null;
                    if (int.TryParse(x, out int num)) result = num;
                    return result;
                }).Where(x => x != null).ToList();

                if (nums.Count == 5)
                {
                    CNMI_ReadResult cNMI_ReadResult = new CNMI_ReadResult();
                    cNMI_ReadResult.Mode = (CNMI_Mode)nums[0];
                    cNMI_ReadResult.MT = (CNMI_MT)nums[1];
                    cNMI_ReadResult.BM = (CNMI_BM)nums[2];
                    cNMI_ReadResult.DS = (CNMI_DS)nums[3];
                    cNMI_ReadResult.BFR = (CNMI_BFR)nums[4];
                    return cNMI_ReadResult;
                }

            }
            return null;
        }
    }

    public class CNMI_ReadResult
    {
        public CNMI_Mode Mode { get; internal set; }
        public CNMI_MT MT { get; internal set; }
        public CNMI_BM BM { get; internal set; }
        public CNMI_DS DS { get; internal set; }
        public CNMI_BFR BFR { get; internal set; }
    }
}
