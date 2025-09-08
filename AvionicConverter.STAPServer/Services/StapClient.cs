using AvionicConverter.Converter.BinaryNumberRepresentation;
using AvionicConverter.Converter.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Timers;
using Timer = System.Timers.Timer;

namespace AvionicConverter.STAPServer.Services;

public class StapClient
{
    public StapClient(TcpClient client)
    {
        networkStream = client.GetStream();
        Client = client;
        _timer = new Timer(TimeSpan.FromSeconds(0.25))
        {
            AutoReset = true,
            Enabled = false,
        };
        _timer.Elapsed += SendDataCallback;

        
        var latitudeConverter = new BnrCompositeConverter();
        latitudeConverter.AddConverter(new(4, 120), BnrConverter.BnrConverterFromRange(0.000172, 11, 9, false), 2);
        latitudeConverter.AddConverter(new(4, 110), BnrConverter.BnrConverterFromRange(180.0, 20, 0, false), 1);
        var longitudeConverter = new BnrCompositeConverter();
        longitudeConverter.AddConverter(new(4, 121), BnrConverter.BnrConverterFromRange(0.000172, 11, 9, false), 2);
        longitudeConverter.AddConverter(new(4, 111), BnrConverter.BnrConverterFromRange(180.0, 20, 0, false), 1);
        var gsConverter = BnrConverter.BnrConverterFromResolution(0.125, 15, 5);
        var trackConverter = BnrConverter.BnrConverterFromResolution(0.0055, 15, 5);

        RequestedData.Add(latitudeConverter.Encode(_latitude, BnrStatusMatrix.NormalOps, new(4, 110)));
        RequestedData.Add(latitudeConverter.Encode(_latitude, BnrStatusMatrix.NormalOps, new(4, 120)));
        RequestedData.Add(longitudeConverter.Encode(_longitude, BnrStatusMatrix.NormalOps, new(4, 111)));
        RequestedData.Add(longitudeConverter.Encode(_longitude, BnrStatusMatrix.NormalOps, new(4, 121)));
        RequestedData.Add(gsConverter.Encode(_groundSpeed, BnrStatusMatrix.NormalOps, new(4, 112)));
        RequestedData.Add(trackConverter.Encode(_track, BnrStatusMatrix.NormalOps, new(4, 103)));
        RequestedData.Add(trackConverter.Encode(_heading, BnrStatusMatrix.NormalOps, new(2, 314)));
    }

    private byte[] Status(string[] ops)
    {
        return Encoding.UTF8.GetBytes("ok\n");
    }

    private byte[] Add(string[] ops)
    {
        Console.WriteLine($"Added {ops[1]} - {ops[2]}");
        RegistererDataRequest(new(ushort.Parse(ops[1]), ushort.Parse(ops[2])));
        return Encoding.UTF8.GetBytes("ok\n");
    }

    public async Task HandleClientAsync()
    {
        var endpoint = Client.Client.RemoteEndPoint?.ToString() ?? "unknown";
        Console.WriteLine($"[Server] Client connected");

        try
        {
            var buffer = new byte[1024];

            while (true)
            {
                int bytesRead = await networkStream.ReadAsync(buffer);
                if (bytesRead == 0)
                {
                    Console.WriteLine($"[Server] Client disconnected: {endpoint}");
                    break;
                }

                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"[Server] Received from {endpoint}: {message}");
                string[] operations = message.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                foreach (var ops in operations)
                {
                    string[] cmds = ops.Split(',');
                    if (cmds[0] == "status") await networkStream.WriteAsync(Status(cmds));
                    else if (cmds[0] == "add") await networkStream.WriteAsync(Add(cmds));
                    else
                    {
                        // Echo back
                        string response = $"Server Echo: {message}";
                        byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                        await networkStream.WriteAsync(responseBytes);
                    }
                }


            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Server] Error with client {endpoint}: {ex.Message}");
        }
        finally
        {
            Client.Close();
        }
    }


    public void RegistererDataRequest(AvionicSource source)
    {
        RequestedParameters.Add(source);
        if (!_timer.Enabled) _timer.Start();
    }

    private void SendDataCallback(object? sender, ElapsedEventArgs e)
    {
        foreach (var data in RequestedData)
        {
            data.TimeStamp = DateTime.UtcNow;
            networkStream.Write(Encoding.UTF8.GetBytes(data.ToString() + "\n"));
            Console.WriteLine(data.ToString());
        }
        Console.WriteLine("\n\n");
    }
    private NetworkStream networkStream;
    public List<AvionicSource> RequestedParameters { get; set; } = [];
    public List<AvionicData> RequestedData { get; set; } = [];
    private readonly Timer _timer;
    public TcpClient Client { get; set; }

    private const double _latitude = 51.11861111;
    private const double _longitude = 4.84222222;
    private const double _groundSpeed = 100.0;
    private const double _track = 0.0;
    private const double _heading = 0.0;
}
