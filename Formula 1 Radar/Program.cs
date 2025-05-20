using System.Threading.Tasks;
using F1R.core.data_acquisition;

class Program
{
    public static int nr = 0;
    public static void Main(string[] args)
    {
        if(nr == 0)
            DataAcq.LiveDataAcq();
        else
        {
            DataAcq.SimulationDataAcq();
        }
        
    }
}




