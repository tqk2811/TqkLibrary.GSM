using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.GSM.Extended
{
    /// <summary>
    /// 
    /// </summary>
    public class CommandRequestCPMS : CommandRequest
    {
        internal CommandRequestCPMS(IGsmClient gsmClient) : base(gsmClient, "CPMS")
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="memr"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<bool> WriteAsync(CPMS_MEMR memr, CancellationToken cancellationToken = default)
            => base.WriteAsync(cancellationToken, memr.ToAtString()).GetTaskResult(x => x.IsSuccess);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="memr"></param>
        /// <param name="memw"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<bool> WriteAsync(CPMS_MEMR memr, CPMS_MEMW memw, CancellationToken cancellationToken = default)
            => base.WriteAsync(cancellationToken, memr.ToAtString(), memw.ToAtString()).GetTaskResult(x => x.IsSuccess);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="memr"></param>
        /// <param name="memw"></param>
        /// <param name="mems"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<bool> WriteAsync(CPMS_MEMR memr, CPMS_MEMW memw, CPMS_MEMS mems, CancellationToken cancellationToken = default)
            => base.WriteAsync(cancellationToken, memr.ToAtString(), memw.ToAtString(), mems.ToAtString()).GetTaskResult(x => x.IsSuccess);


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
    }
    /// <summary>
    /// 
    /// </summary>
    public static class CommandRequestCPMSExtension
    {
        /// <summary>
        /// Preferred Message Storage
        /// </summary>
        /// <param name="gsmClient"></param>
        /// <returns></returns>
        public static CommandRequestCPMS CPMS(this IGsmClient gsmClient) => new CommandRequestCPMS(gsmClient);
    }
}
