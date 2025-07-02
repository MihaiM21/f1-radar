namespace F1R.core.storage_cache;
using System.ComponentModel.DataAnnotations;

public class Driver
{
    [Key]
    public int DriverNumber { get; set; }
    public (int X, int Y, int Z) Position { get; set; }
    public double BestLap { get; set; }
    public double LastLap { get; set; }
    public (double S1, double S2, double S3) SectorTimes { get; set; }
    public (string S1, string S2, string S3) SectorStatus { get; set; }
    public (string s1, string s2, string s3, string s4, string s5, string s6, string s7) S1SegmentStatus { get; set; }
    public (string s1, string s2, string s3, string s4, string s5, string s6, string s7) S2SegmentStatus { get; set; }
    public (string s1, string s2, string s3, string s4, string s5, string s6, string s7) S3SegmentStatus { get; set; }
    public List<int> Sector1MiniSectors { get; set; } = new();
    public List<int> Sector2MiniSectors { get; set; } = new();
    public List<int> Sector3MiniSectors { get; set; } = new();
    public double SpeedTrapSpeed { get; set; }
}