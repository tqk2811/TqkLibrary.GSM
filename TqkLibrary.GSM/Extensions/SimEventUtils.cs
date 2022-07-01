using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace TqkLibrary.GSM.Extensions
{
    public class SimEventUtils : IDisposable
    {
        public event Action OnSimPlugIn;
        public event Action OnSimPlugOut;
        public event Action OnProviderConnected;
        readonly GsmClient gsmClient;
        public SimEventUtils(GsmClient gsmClient)
        {
            this.gsmClient = gsmClient ?? throw new ArgumentNullException(nameof(gsmClient));
            gsmClient.OnUnknowReceived += GsmClient_OnUnknowReceived;
            gsmClient.OnCommandResponse += GsmClient_OnCommandResponse;
        }
        ~SimEventUtils()
        {
            Dispose(false);
        }
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




        private void GsmClient_OnCommandResponse(GsmCommandResponse obj)
        {
            if ("CPIN".Equals(obj?.Command))
            {
                var arg = obj.Arguments.FirstOrDefault()?.Trim('"');
                switch(arg)
                {
                    case "NOT READY":
                        ThreadPool.QueueUserWorkItem((o) => OnSimPlugOut?.Invoke());
                        break;

                    case "READY":
                        ThreadPool.QueueUserWorkItem((o) => OnSimPlugIn?.Invoke());
                        break;
                }
            }
        }

        private void GsmClient_OnUnknowReceived(string obj)
        {
            switch (obj)
            {
                case "Call Ready":
                    ThreadPool.QueueUserWorkItem((o) => OnProviderConnected?.Invoke());
                    break;
            }
        }
    }
}
