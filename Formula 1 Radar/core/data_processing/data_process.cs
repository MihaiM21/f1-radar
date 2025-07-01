using System.Text.Json;
using F1R.core.storage_cache;
using F1R.core.data_processing;
using Microsoft.AspNetCore.Mvc.Diagnostics;

namespace F1R.core.data_processing;

public class data_process
{
    
    public static void processData(string inputData)
    {
        var document = JsonDocument.Parse(inputData);
        // Console.WriteLine(inputData);
        // Console.WriteLine(document.RootElement.GetProperty("A").GetArrayLength());
        string type = document.RootElement.GetProperty("A")[0].GetString();
        
        switch (type)
        {
            case "Position.z":
                // Decode first
                string decodedPositionData = decoder.decodeMessage(inputData);
                PositionData(decodedPositionData);
                break;
            case "CarData.z":
                // Decode first
                string decodedCarData = decoder.decodeMessage(inputData);
                CarData(decodedCarData);
                
                break;
            case "TimingData":
                // Process Timing Data
                TimingData(inputData);
                
                break;
            case "LapData":
                // Process Lap Data
                
                break;
            default:
                Console.WriteLine($"Unknown data type: {type}");
                break;
        }
    }
    
    private static void PositionData(string inputData)
    {
        
        try
        {
            var document = JsonDocument.Parse(inputData);
            var positions = document.RootElement.GetProperty("Position");

            foreach (var position in positions.EnumerateArray())
            {
                var entries = position.GetProperty("Entries");
                foreach (var entry in entries.EnumerateObject())
                {
                    string key = entry.Name; // Driver Number
                    // string to int
                    int number = int.Parse(key);
                    var entryData = entry.Value;
                    
                    string status = entryData.GetProperty("Status").GetString();
                    int x = entryData.GetProperty("X").GetInt32();
                    int y = entryData.GetProperty("Y").GetInt32();
                    int z = entryData.GetProperty("Z").GetInt32();
                    
                    // ADD SAVE OPTION
                    // _driverManager.UpdateDriverPartial(number, d =>
                    // {
                    //     d.Position = (x, y, z);
                    // });
                    // Console.WriteLine(_driverManager.GetDriver(number).Position);
                    
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private static void CarData(string inputData)
    {
        try
        {
            var document = JsonDocument.Parse(inputData);
            // Console.WriteLine(inputData);
            var entries = document.RootElement.GetProperty("Entries");

            foreach (var entry in entries.EnumerateArray())
            {
                var data = entry.GetProperty("Cars");
                foreach (var driverData in data.EnumerateObject())
                {
                    string key = driverData.Name; // Driver Number
                    int driverNumber = int.Parse(key);
                    // Console.WriteLine($"Driver: {driverNumber}");
                    var carsData = driverData.Value;

                    foreach (var carData in carsData.EnumerateObject())
                    {
                        // Console.WriteLine($"Car: {carData}");
                        var cars = carData.Value;

                        // Process each car's data
                        int rpm = cars.GetProperty("0").GetInt32();
                        int speed = cars.GetProperty("2").GetInt32();
                        int gear = cars.GetProperty("3").GetInt32();
                        int throttle = cars.GetProperty("4").GetInt32();
                        int brake = cars.GetProperty("5").GetInt32();
                        int drs = cars.GetProperty("45").GetInt32();
                        
                        // ADD SAVE OPTION here
                    }

                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private static void TimingData(string inputData)
    {
        try
        {
            var document = JsonDocument.Parse(inputData);
            // Console.WriteLine(inputData);
            var entries = document.RootElement.GetProperty("A")[1];
            // Console.WriteLine(entries);
            foreach (var entry in entries.EnumerateObject())
            {
                var key = entry.Name; // Lines
                var data = entry.Value; //Data
                foreach (var driversData in data.EnumerateObject())
                {
                    var driverNumber = driversData.Name;
                    var allDriverData = driversData.Value;
                    // Poate avea date despre pit stop TREBUIE FACUT SI PENTRU ALEA
                    try
                    {
                        foreach (var driverData in allDriverData.EnumerateObject())
                        {
                            // Checking what type of data has
                            var dataType = driverData.Name;
                            // Taking all of the data
                            var dataResult = driverData.Value;

                            switch (dataType)
                            {
                                case "Sectors":
                                    foreach (var sector in dataResult.EnumerateObject())
                                    {
                                        // Get the sector number
                                        var sectorNumber = sector.Name;
                                        // Sector Data
                                        var sectorsData = sector.Value;
                            
                                        foreach (var sectorData in sectorsData.EnumerateObject())
                                        {
                                            // Checking the sector data type in a switch case
                                            var sectorDataType = sectorData.Name;
                                            var sectorDataValue = sectorData.Value;

                                            switch (sectorDataType)
                                            {
                                                case "Segments":
                                                    foreach (var segment in sectorDataValue.EnumerateObject())
                                                    {
                                                        // Segment Number
                                                        var segmentNumber = segment.Name;
                                                        // Segment Status (2049, 2050, etc.)
                                                        var segmentStatus = segment.Value.GetProperty("Status");
                                                        Console.WriteLine(sectorNumber);
                                                        Console.WriteLine(segmentStatus);
                                                    }
                                                    break;
                                                case "Status":

                                                    break;
                                                default:
                                                    Console.WriteLine($"Unknown sector data type: {dataType}");
                                                    break;
                                            }

                                        }
                                    }
                                    break;
                                case "Speeds":

                                    break;
                                
                                default:
                                    Console.WriteLine($"Unknown timing data type: {dataType}");
                                    break;
                            }
                            
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}