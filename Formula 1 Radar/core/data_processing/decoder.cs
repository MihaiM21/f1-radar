using System;
using System.IO.Compression;
using System.Text;
using System.Text.Json;

namespace F1R.core.data_processing;

public class decoder
{
    public static string decodeMessage(string data)
    {
        try
        {
            // Attempt to parse as JSON
            var document = JsonDocument.Parse(data);
            string encodedData = document.RootElement.GetProperty("A")[1].GetString();

            // Decode Base64 and decompress
            var base64Decoded = Convert.FromBase64String(encodedData);
            using var decompressionStream = new DeflateStream(new MemoryStream(base64Decoded), CompressionMode.Decompress);
            using var reader = new StreamReader(decompressionStream, Encoding.UTF8);
            return reader.ReadToEnd(); // Return the decompressed string
        }
        catch
        {
            // If JSON parsing or decompression fails, return the raw data
            return data;
        }
    }
}