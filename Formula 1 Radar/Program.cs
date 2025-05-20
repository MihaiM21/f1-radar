using System.Threading.Tasks;
using F1R.core.data_acquisition;
using F1R.core.simulation;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.Cors;
using Owin;

namespace F1R
{
    class Program
    {
        public static int nr = 1;
        public static async Task Main(string[] args)
        {
            var signalRServer = new SignalRServer();
            //signalRServer.Start();
        
            if(nr == 0)
                DataAcq.LiveDataAcq();
            else
            {
                await DataAcq.SimulationDataAcq();
            }
        
        }
    }
}





