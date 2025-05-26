namespace F1R.core.storage_cache;

public class Driver
{
    public int DriverNumber { get; set; }
    public (int X, int Y, int Z) Position { get; set; }
    public double BestLap { get; set; }
    public double LastLap { get; set; }
    public double Sector1Time { get; set; }
    public double Sector2Time { get; set; }
    public double Sector3Time { get; set; }
    public List<int> Sector1MiniSectors { get; set; } = new();
    public List<int> Sector2MiniSectors { get; set; } = new();
    public List<int> Sector3MiniSectors { get; set; } = new();
    public double SpeedTrapSpeed { get; set; }
}