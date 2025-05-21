using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using F1R.core.data_processing;

namespace F1R.core.data_acquisition
{
    public class DataAcq
    {
        static CancellationTokenSource quitTokenSource = new CancellationTokenSource();

        
        public static async void LiveDataAcq()
        {
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                quitTokenSource.Cancel();
                eventArgs.Cancel = true;
            };

            Console.WriteLine("Starting");

            // Getting the log file ready
            var logFilePath = "livetiming_log.txt";
            using var logWriter = new StreamWriter(logFilePath, append: true, encoding: Encoding.UTF8)
            {
                AutoFlush = true
            };

            // URL for SignalR (F1)
            using var connection = new HubConnection("https://livetiming.formula1.com/signalr");
            connection.CookieContainer = new CookieContainer();

            // Writing in console and in the file the data received
            connection.Received += async data =>
            {
                Console.WriteLine("Recv: " + data);
                await logWriter.WriteLineAsync($"{data}");
            };

            connection.StateChanged += change =>
            {
                Console.WriteLine("Connection state changed to " + change.NewState);
                logWriter.WriteLine($"[{DateTime.UtcNow:O}] State changed to: {change.NewState}");
            };

            connection.ConnectionSlow += () =>
            {
                Console.WriteLine("Connection is slow");
                logWriter.WriteLine($"[{DateTime.UtcNow:O}] Connection is slow");
            };

            connection.Error += ex =>
            {
                Console.WriteLine("The following connection error was encountered: " + ex.Message);
                logWriter.WriteLine($"[{DateTime.UtcNow:O}] Error: {ex}");
            };

            connection.Reconnecting += () =>
            {
                Console.WriteLine("Reconnecting...");
                logWriter.WriteLine($"[{DateTime.UtcNow:O}] Reconnecting...");
            };

            connection.Reconnected += () =>
            {
                Console.WriteLine("Reconnected");
                logWriter.WriteLine($"[{DateTime.UtcNow:O}] Reconnected");
            };

            var streamingHub = connection.CreateHubProxy("Streaming");
            await connection.Start();

            // Adding the interest Channels
            await streamingHub.Invoke("Subscribe", new List<string> {
                "Heartbeat",
                "CarData.z",
                "Position.z",
                "ExtrapolatedClock",
                "TopThree",
                "RcmSeries",
                "TimingStats",
                "TimingAppData",
                "WeatherData",
                "TrackStatus",
                "SessionStatus",
                "DriverList",
                "RaceControlMessages",
                "SessionInfo",
                "SessionData",
                "LapCount",
                "TimingData",
                "TeamRadio",
                "PitLaneTimeCollection",
                "ChampionshipPrediction"
            });

            quitTokenSource.Token.WaitHandle.WaitOne();
            connection.Stop();

            Console.WriteLine("Quitting");
            logWriter.WriteLine($"[{DateTime.UtcNow:O}] Quitting");
        }
        public static async Task SimulationDataAcq()
        {
            var uri = new Uri("ws://localhost:5000");
            using var client = new ClientWebSocket();

            try
            {
                Console.WriteLine("Connecting to Python WebSocket server...");
                await client.ConnectAsync(uri, CancellationToken.None);
                Console.WriteLine("Connected!");

                var buffer = new byte[8192];

                while (client.State == WebSocketState.Open)
                {
                    var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        Console.WriteLine("Connection closed by server.");
                        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", CancellationToken.None);
                    }
                    else
                    {
                        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        Console.WriteLine($"Received: {message}");
                        string decodedData = decoder.decodeMessage(message);
                        //Console.WriteLine("Decoded: " + decodedData);
                        string prettyData = data_pretty.separate(decodedData);
                        Console.WriteLine("Pretty: " + prettyData);
                        string separateEntries = data_pretty.separateEntries(decodedData);
                        Console.WriteLine("Separate: " + separateEntries);
                    }
                }
            }
            catch (WebSocketException wse)
            {
                Console.WriteLine("WebSocket error: " + wse.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Other error: " + ex.Message);
            }           
            
            // Console.CancelKeyPress += (sender, eventArgs) =>
            // {
            //     quitTokenSource.Cancel();
            //     eventArgs.Cancel = true;
            // };
            //
            // Console.WriteLine("Starting");
            //
            // // Getting the log file ready
            // var logFilePath = "test_livetiming_log.txt";
            // using var logWriter = new StreamWriter(logFilePath, append: true, encoding: Encoding.UTF8)
            // {
            //     AutoFlush = true
            // };
            //
            // // URL for SignalR (F1)
            // using var connection = new HubConnection("http://localhost:5000/signalr");
            // connection.CookieContainer = new CookieContainer();
            //
            // // Writing in console and in the file the data received
            // connection.Received += async data =>
            // {
            //     Console.WriteLine("Recv: " + data);
            //     await logWriter.WriteLineAsync($"[{DateTime.UtcNow:O}] {data}");
            // };
            //
            // connection.StateChanged += change =>
            // {
            //     Console.WriteLine("Connection state changed to " + change.NewState);
            //     logWriter.WriteLine($"[{DateTime.UtcNow:O}] State changed to: {change.NewState}");
            // };
            //
            // connection.ConnectionSlow += () =>
            // {
            //     Console.WriteLine("Connection is slow");
            //     logWriter.WriteLine($"[{DateTime.UtcNow:O}] Connection is slow");
            // };
            //
            // connection.Error += ex =>
            // {
            //     Console.WriteLine("The following connection error was encountered: " + ex.Message);
            //     logWriter.WriteLine($"[{DateTime.UtcNow:O}] Error: {ex}");
            // };
            //
            // connection.Reconnecting += () =>
            // {
            //     Console.WriteLine("Reconnecting...");
            //     logWriter.WriteLine($"[{DateTime.UtcNow:O}] Reconnecting...");
            // };
            //
            // connection.Reconnected += () =>
            // {
            //     Console.WriteLine("Reconnected");
            //     logWriter.WriteLine($"[{DateTime.UtcNow:O}] Reconnected");
            // };
            //
            // var streamingHub = connection.CreateHubProxy("Streaming");
            // await connection.Start();
            //
            // // Adding the interest Channels
            // await streamingHub.Invoke("Subscribe", new List<string> {
            //     "Heartbeat",
            //     "CarData.z",
            //     "Position.z",
            //     "ExtrapolatedClock",
            //     "TopThree",
            //     "RcmSeries",
            //     "TimingStats",
            //     "TimingAppData",
            //     "WeatherData",
            //     "TrackStatus",
            //     "SessionStatus",
            //     "DriverList",
            //     "RaceControlMessages",
            //     "SessionInfo",
            //     "SessionData",
            //     "LapCount",
            //     "TimingData",
            //     "TeamRadio",
            //     "PitLaneTimeCollection",
            //     "ChampionshipPrediction"
            // });
            //
            // quitTokenSource.Token.WaitHandle.WaitOne();
            // connection.Stop();
            //
            // Console.WriteLine("Quitting");
            // logWriter.WriteLine($"[{DateTime.UtcNow:O}] Quitting");
        }
    }
}