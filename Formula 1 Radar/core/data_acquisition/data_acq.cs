﻿using Microsoft.AspNet.SignalR.Client;
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
using F1R.core.storage_cache;

namespace F1R.core.data_acquisition
{
    public class DataAcq
    {
        static CancellationTokenSource quitTokenSource = new CancellationTokenSource();
        private static readonly object fileLock = new object();
        // Define an event to broadcast the data
        //public static event Action<string> OnDataReceived;
        
        public static async Task LiveDataAcq()
        {
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                quitTokenSource.Cancel();
                eventArgs.Cancel = true;
            };

            Console.WriteLine("Starting");

            var logFilePath = "practice1_livetiming_log.txt";
            var decryptedFilePath = "practice1_livetiming_decrypted.txt";
            var decryptedFilePathSeparate = "practice1_livetiming_decrypted_separated.txt";

            using var logWriter = new StreamWriter(logFilePath, append: true, encoding: Encoding.UTF8) { AutoFlush = true };
            using var decryptedWriter = new StreamWriter(decryptedFilePath, append: true, encoding: Encoding.UTF8) { AutoFlush = true };
            using var decryptedWriterSeparate = new StreamWriter(decryptedFilePathSeparate, append: true, encoding: Encoding.UTF8) { AutoFlush = true };

            var connection = new HubConnection("https://livetiming.formula1.com/signalr");
            connection.CookieContainer = new CookieContainer();

            connection.Received += async data =>
            {
                try
                {
                    //OnDataReceived?.Invoke(data);
                    // await logWriter.WriteLineAsync($"{data}");
                    // string decodedData = decoder.decodeMessage(data); // May throw
                    // string prettyData = data_pretty.separate(decodedData);
                    // string separateEntries = data_pretty.separateEntries(decodedData);
                    // await decryptedWriter.WriteLineAsync($"{prettyData}");
                    // await decryptedWriterSeparate.WriteLineAsync($"{separateEntries}");
                    string decodedData = decoder.decodeMessage(data);
                    string prettyData = data_pretty.separate(decodedData);
                    string separateEntries = data_pretty.separateEntries(decodedData);
                    Console.WriteLine(decodedData);
                    lock (fileLock)
                    {
                        
                        logWriter.WriteLine(data);
                        decryptedWriter.WriteLine(prettyData);
                        decryptedWriterSeparate.WriteLine(separateEntries);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Error in data processing] {ex.Message}");
                    await logWriter.WriteLineAsync($"[{DateTime.UtcNow:O}] Error in Received handler: {ex}");
                }
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

            // Keep retrying until connected or user cancels
            while (!quitTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    await connection.Start();
                    Console.WriteLine("Connected to SignalR.");
                    logWriter.WriteLine($"[{DateTime.UtcNow:O}] Connected to SignalR.");
                    break; // Exit loop if connected
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to connect. Retrying in 5 seconds...");
                    logWriter.WriteLine($"[{DateTime.UtcNow:O}] Failed to connect: {ex.Message}");
                    await Task.Delay(5000, quitTokenSource.Token); // Wait before retry
                }
            }

            // If cancel token triggered during wait, stop execution
            if (quitTokenSource.Token.IsCancellationRequested)
            {
                Console.WriteLine("Cancelled before connection was established.");
                return;
            }

            // Add interest Channels
            try
            {
                await streamingHub.Invoke("Subscribe", new List<string> {
                    "Heartbeat", "CarData.z", "Position.z", "ExtrapolatedClock", "TopThree", "RcmSeries",
                    "TimingStats", "TimingAppData", "WeatherData", "TrackStatus", "SessionStatus",
                    "DriverList", "RaceControlMessages", "SessionInfo", "SessionData", "LapCount",
                    "TimingData", "TeamRadio", "PitLaneTimeCollection", "ChampionshipPrediction"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Subscribe error] {ex.Message}");
                await logWriter.WriteLineAsync($"[{DateTime.UtcNow:O}] Error subscribing: {ex}");
            }

            quitTokenSource.Token.WaitHandle.WaitOne(); // Wait for cancel signal
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
                        // Process data
                        data_process.processData(message);
                        
                        // UPDATE DRIVER FUNCTION HERE
                        //Console.WriteLine($"TEST: {DriversManager.GetAllDrivers()}");

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