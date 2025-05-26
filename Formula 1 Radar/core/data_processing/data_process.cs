using System.Text.Json;
using F1R.core.storage_cache;
using F1R.core.data_processing;
namespace F1R.core.data_processing;

public class data_process
{
    public static DriverManager _driverManager = new DriverManager("./drivers");
    
    public static void processData(string inputData)
    {
        var document = JsonDocument.Parse(inputData);
        Console.WriteLine(inputData);
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
            // Console.WriteLine(inputData);
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
            Console.WriteLine(inputData);
            var entries = document.RootElement.GetProperty("Entries");

            foreach (var entry in entries.EnumerateArray())
            {
                var data = entry.GetProperty("Cars");
                foreach (var driverData in data.EnumerateObject())
                {
                    string key = driverData.Name; // Driver Number
                    int driverNumber = int.Parse(key);
                    Console.WriteLine($"Driver: {driverNumber}");
                    var carsData = driverData.Value;

                    foreach (var carData in carsData.EnumerateObject())
                    {
                        Console.WriteLine($"Car: {carData}");
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
}