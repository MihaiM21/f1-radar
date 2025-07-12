using System.Threading.Tasks;
using F1R.core.data_acquisition;
using F1R.core.simulation;
using F1R.core.hubs;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.Cors;
using Owin;

namespace F1R
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                await DataAcq.LiveDataAcq();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        
        }
    }
}





