namespace F1R.core.storage_cache;

public class DataManager
{
    private static readonly Dictionary<int, Driver> drivers = new();

    // Cheama functia la inceputul rularii aplicatiei NU UITA !!!!
    public static void EnsureDriverExists(int driverNumber)
    {
        if (!drivers.ContainsKey(driverNumber))
        {
            drivers[driverNumber] = new Driver
            {
                DriverNumber = driverNumber,
                Position = (0, 0, 0),
                BestLap = 0.0,
                LastLap = 0.0,
                SectorTimes = (0, 0, 0),
                SectorStatus = ("Yellow", "Yellow", "Yellow"),
                S1SegmentStatus = ("Yellow", "Yellow", "Yellow", "Yellow", "Yellow", "Yellow", "Yellow"),
                S2SegmentStatus = ("Yellow", "Yellow", "Yellow", "Yellow", "Yellow", "Yellow", "Yellow"),
                S3SegmentStatus = ("Yellow", "Yellow", "Yellow", "Yellow", "Yellow", "Yellow", "Yellow"),
                SpeedTrapSpeed = 0.0
            };
        }
    }

    public static Driver GetDriver(int driverNumber)
    {
        EnsureDriverExists(driverNumber);
        if (drivers.TryGetValue(driverNumber, out var driver))
            return driver;
        return null;
    }

    public static void UpdateBestLap(int driverNumber, double bestLap)
    {
        EnsureDriverExists(driverNumber);
        if (drivers.ContainsKey(driverNumber))
            drivers[driverNumber].BestLap = bestLap;
    }

    public static void UpdatePosition(int driverNumber, int x, int y, int z)
    {
        EnsureDriverExists(driverNumber);
        if (drivers.TryGetValue(driverNumber, out var driver))
            driver.Position = (x, y, z);
    }

    public static void UpdateLastLap(int driverNumber, double lastLap)
    {
        EnsureDriverExists(driverNumber);
        if (drivers.ContainsKey(driverNumber))
        {
            drivers[driverNumber].LastLap = lastLap;
        }
    }
    
    public static void UpdateSectorTimes(int driverNumber, (double, double, double) sectorTimes)
    {
        EnsureDriverExists(driverNumber);
        if (drivers.ContainsKey(driverNumber))
        {
            drivers[driverNumber].SectorTimes = sectorTimes;
        }
    }
    
    public static void UpdateSectorStatus(int driverNumber, (string, string, string) sectorStatus)
    {
        EnsureDriverExists(driverNumber);
        if (drivers.ContainsKey(driverNumber))
        {
            drivers[driverNumber].SectorStatus = sectorStatus;
        }
    }
    
    public static void UpdateS1SegmentStatus(int driverNumber, (string, string, string, string, string, string, string) s1SegmentStatus)
    {
        EnsureDriverExists(driverNumber);
        if (drivers.ContainsKey(driverNumber))
        {
            drivers[driverNumber].S1SegmentStatus = s1SegmentStatus;
        }
    }
    
    public static void UpdateS2SegmentStatus(int driverNumber, (string, string, string, string, string, string, string) s2SegmentStatus)
    {
        EnsureDriverExists(driverNumber);
        if (drivers.ContainsKey(driverNumber))
        {
            drivers[driverNumber].S2SegmentStatus = s2SegmentStatus;
        }
    }
    
    public static void UpdateS3SegmentStatus(int driverNumber, (string, string, string, string, string, string, string) s3SegmentStatus)
    {
        EnsureDriverExists(driverNumber);
        if (drivers.ContainsKey(driverNumber))
        {
            drivers[driverNumber].S3SegmentStatus = s3SegmentStatus;
        }
    }
}