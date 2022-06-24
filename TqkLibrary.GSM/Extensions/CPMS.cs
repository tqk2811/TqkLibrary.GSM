using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.GSM.Extensions
{
    /// <summary>
    /// memory from which messages are read and deleted
    /// </summary>
    public enum CPMS_MEMR
    {
        /// <summary>
        /// SIM SMS memory storage 
        /// </summary>
        SM,
        /// <summary>
        /// ME internal storage (read only, no delete)
        /// </summary>
        ME
    }
    /// <summary>
    /// memory to which writing and sending operations are made
    /// </summary>
    public enum CPMS_MEMW
    {
        /// <summary>
        /// SIM SMS memory storage
        /// </summary>
        SM,
    }

    /// <summary>
    /// memory to which received SMs are preferred to be stored
    /// </summary>
    public enum CPMS_MEMS
    {
        /// <summary>
        /// SIM SMS memory storage
        /// </summary>
        SM,
    }


    public static partial class GsmExtensions
    {
        /// <summary>
        /// Preferred Message Storage
        /// </summary>
        /// <returns></returns>
        public static Task<bool> WritePreferredMessageStorage(this GsmClient gsmClient,
            CPMS_MEMR memr,
            CancellationToken cancellationToken = default)
            => gsmClient.Write("CPMS", cancellationToken, memr.ToAtString())
            .GetTaskResult(x => x.IsSuccess);

        /// <summary>
        /// Preferred Message Storage
        /// </summary>
        /// <returns></returns>
        public static Task<bool> WritePreferredMessageStorage(this GsmClient gsmClient,
            CPMS_MEMR memr,
            CPMS_MEMW memw,
            CancellationToken cancellationToken = default)
            => gsmClient.Write("CPMS", cancellationToken, memr.ToAtString(), memw.ToAtString())
            .GetTaskResult(x => x.IsSuccess);

        /// <summary>
        /// Preferred Message Storage
        /// </summary>
        /// <returns></returns>
        public static Task<bool> WritePreferredMessageStorage(this GsmClient gsmClient,
            CPMS_MEMR memr,
            CPMS_MEMW memw,
            CPMS_MEMS mems,
            CancellationToken cancellationToken = default)
            => gsmClient.Write("CPMS", cancellationToken, memr.ToAtString(), memw.ToAtString(), mems.ToAtString())
            .GetTaskResult(x => x.IsSuccess);

    }
}
