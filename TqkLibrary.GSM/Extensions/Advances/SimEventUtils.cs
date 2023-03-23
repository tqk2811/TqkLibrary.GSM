using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace TqkLibrary.GSM.Extensions.Advances
{
    /// <summary>
    /// 
    /// </summary>
    public static class SimEventUtilsExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gsmClient"></param>
        /// <returns></returns>
        public static SimEventUtils RegisterSimEventUtils(this GsmClient gsmClient) => new SimEventUtils(gsmClient);
    }
    /// <summary>
    /// 
    /// </summary>
    public class SimEventUtils : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        public event Action OnSimPlugIn;
        /// <summary>
        /// 
        /// </summary>
        public event Action OnSimPlugOut;
        /// <summary>
        /// 
        /// </summary>
        public event Action OnProviderConnected;
        /// <summary>
        /// 
        /// </summary>
        public event Action<CallingHelper> OnCalling;
        /// <summary>
        /// 
        /// </summary>
        public event Action<CallingHelper> OnCallingClip;
        /// <summary>
        /// 
        /// </summary>
        public event Action OnEndCall;



        readonly GsmClient gsmClient;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gsmClient"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public SimEventUtils(GsmClient gsmClient)
        {
            this.gsmClient = gsmClient ?? throw new ArgumentNullException(nameof(gsmClient));
            gsmClient.OnUnknowReceived += GsmClient_OnUnknowReceived;
            gsmClient.OnCommandResponse += GsmClient_OnCommandResponse;
        }
        /// <summary>
        /// 
        /// </summary>
        ~SimEventUtils()
        {
            Dispose(false);
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            gsmClient.OnUnknowReceived -= GsmClient_OnUnknowReceived;
            gsmClient.OnCommandResponse -= GsmClient_OnCommandResponse;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task EnableClip(CancellationToken cancellationToken = default)
        {
            await gsmClient.CLIP().WriteAsync(CommandRequestCLIP.CliPresentation.Enable, cancellationToken);
        }


        private void GsmClient_OnCommandResponse(GsmCommandResponse obj)
        {
            switch (obj?.Command)
            {
                case "CPIN":
                    {
                        var arg = obj.Arguments.FirstOrDefault()?.Trim('"');
                        switch (arg)
                        {
                            case "NOT READY":
                                if (OnSimPlugOut != null) ThreadPool.QueueUserWorkItem((o) => OnSimPlugOut?.Invoke());
                                break;

                            case "READY":
                                if (OnSimPlugIn != null) ThreadPool.QueueUserWorkItem((o) => OnSimPlugIn?.Invoke());
                                break;
                        }
                        break;
                    }

                case "CLIP":
                    {
                        if (OnCallingClip != null) ThreadPool.QueueUserWorkItem((o) => OnCallingClip?.Invoke(new CallingHelper(gsmClient, this, obj.Arguments.FirstOrDefault())));
                        break;
                    }
            }
        }

        private void GsmClient_OnUnknowReceived(string obj)
        {
            switch (obj)
            {
                case "Call Ready":
                    if (OnProviderConnected != null) ThreadPool.QueueUserWorkItem((o) => OnProviderConnected?.Invoke());
                    break;
                case "RING":
                    if (OnCalling != null) ThreadPool.QueueUserWorkItem((o) => OnCalling?.Invoke(new CallingHelper(gsmClient, this)));
                    break;
                case "NO CARRIER":
                    if (OnEndCall != null) ThreadPool.QueueUserWorkItem((o) => OnEndCall?.Invoke());
                    break;
            }
        }
    }
}
