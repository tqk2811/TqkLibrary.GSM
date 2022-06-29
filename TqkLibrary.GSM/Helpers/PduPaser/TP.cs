/*
Original https://github.com/wi1dcard/sms-decoder
Edit: tqk2811
 */
using System;

namespace TqkLibrary.GSM.Helpers.PduPaser
{
    /// <summary>
    /// http://subnets.ru/saved/sms_submit_fo.html <br>
    /// </br>https://en.wikipedia.org/wiki/GSM_03.40
    /// </summary>
    [Flags]
    public enum TP : byte
    {
        None = 0,
        /// <summary>
        /// Message Type Indicator bit1
        /// </summary>
        MTI = 1 << 0,
        /// <summary>
        /// Message Type Indicator bit2
        /// </summary>
        MTI2 = 1 << 1,
        /// <summary>
        /// Reject Duplicates
        /// </summary>
        RD = 1 << 2,
        /// <summary>
        /// Validity-Period-Format
        /// </summary>
        VPF = 1 << 3,
        /// <summary>
        /// Validity-Period-Format2
        /// </summary>
        VPF2 = 1 << 4,
        /// <summary>
        /// Status-Report-Request
        /// </summary>
        SRR = 1 << 5,
        /// <summary>
        /// User-Data-Header-Indicator
        /// </summary>
        UDHI = 1 << 6,
        /// <summary>
        /// Reply-Path
        /// </summary>
        RP = 1 << 7,
    }
}
