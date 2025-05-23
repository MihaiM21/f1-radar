using System.Text.Json;

namespace F1R.core.data_processing;

public class data_pretty
{
      public static string separate(string inputData)
      {
          
          //This method takes a string input that MUST be decrypted before and separates the data from each timestamp
          // Pretty print the data
          try
          {
              // Parse the input JSON string
              var jsonElement = JsonDocument.Parse(inputData).RootElement;

              // Serialize it back with indented formatting
              var options = new JsonSerializerOptions
              {
                  WriteIndented = true
              };

              return JsonSerializer.Serialize(jsonElement, options);
          }
          catch
          {
              // Return the original data if parsing fails
              return inputData;
          }
          
     }

      public static string separateEntries(string inputData)
      {
          try
          {
              // This method takes a string that is already separated and separates it even more for simpler use.
              // Parse the input JSON string
              var document = JsonDocument.Parse(inputData);
              var positions = document.RootElement.GetProperty("Position");

              var result = new List<string>();

              foreach (var position in positions.EnumerateArray())
              {
                  string timestamp = position.GetProperty("Timestamp").GetString();
                  result.Add($"Timestamp: {timestamp}");

                  var entries = position.GetProperty("Entries");
                  foreach (var entry in entries.EnumerateObject())
                  {
                      string key = entry.Name;
                      var entryData = entry.Value;

                      string status = entryData.GetProperty("Status").GetString();
                      int x = entryData.GetProperty("X").GetInt32();
                      int y = entryData.GetProperty("Y").GetInt32();
                      int z = entryData.GetProperty("Z").GetInt32();

                      result.Add($"Key: {key}, Status: {status}, X: {x}, Y: {y}, Z: {z}");
                  }
              }

              // Join all entries into a single formatted string
              return string.Join("\n", result);
          }
          catch
          {
              return inputData; // Return the original data if parsing fails
          }
      }

}