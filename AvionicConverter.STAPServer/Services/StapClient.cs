using AvionicConverter.Converter.BinaryNumberRepresentation;
using AvionicConverter.Converter.Models;
using AvionicConverter.STAPServer.Messages;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Timers;
using Timer = System.Timers.Timer;

namespace AvionicConverter.STAPServer.Services;

public class StapClient : IRecipient<GpsDataMessage>
{
    public StapClient(TcpClient client) 
    {
        WeakReferenceMessenger.Default.Register(this);  
        networkStream = client.GetStream();
        Client = client;

        var latitudeConverter = new BnrCompositeConverter();
        latitudeConverter.AddConverter(new(4, 120), BnrConverter.BnrConverterFromRange(0.000172, 11, 9, false), 2);
        latitudeConverter.AddConverter(new(4, 110), BnrConverter.BnrConverterFromRange(180.0, 20, 0, false), 1);
        var longitudeConverter = new BnrCompositeConverter();
        longitudeConverter.AddConverter(new(4, 121), BnrConverter.BnrConverterFromRange(0.000172, 11, 9, false), 2);
        longitudeConverter.AddConverter(new(4, 111), BnrConverter.BnrConverterFromRange(180.0, 20, 0, false), 1);
        var gsConverter = BnrConverter.BnrConverterFromResolution(0.125, 15, 5);
        var trackConverter = BnrConverter.BnrConverterFromResolution(0.0055, 15, 5);

        Converters.Add(new(4, 110), latitudeConverter);
        Converters.Add(new(4, 111), longitudeConverter);
        Converters.Add(new(4, 120), latitudeConverter);
        Converters.Add(new(4, 121), longitudeConverter);
        Converters.Add(new(4, 112), gsConverter);
        Converters.Add(new(4, 103), trackConverter);
        Converters.Add(new(2, 314), trackConverter);
    }

    private byte[] Status(string[] ops)
    {
        return Encoding.UTF8.GetBytes("ok\n");
    }

    private byte[] Add(string[] ops)
    {
        AvionicSource source = new(ushort.Parse(ops[1]), ushort.Parse(ops[2]));
        Console.WriteLine($"Added {ops[1]} - {ops[2]}");

        RequestedParameters.Add(source);

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


    public void UpdatePosition(GpsData data)
    {
        Values[new(4, 110)] = data.Latitude;
        Values[new(4, 111)] = data.Longitude;
        Values[new(4, 120)] = data.Latitude;
        Values[new(4, 121)] = data.Longitude;
        Values[new(4, 112)] = data.Groundspeed;
        Values[new(4, 103)] = data.Track;
        Values[new(2, 314)] = data.Heading;

        Console.WriteLine($"Position Update : {data.Latitude}-{data.Longitude}");
        SendDataCallback();
    }

    private void SendDataCallback()
    {
        foreach (AvionicSource source in RequestedParameters)
        {
            AvionicData res;
            if (Values[new(4, 112)] < 1.0 && source.Equals(new(4, 103)))
            {
                res = Converters[source].Encode(Values[source], BnrStatusMatrix.NoComputedData, source);
            }
            else
            {
                res = Converters[source].Encode(Values[source], BnrStatusMatrix.NormalOps, source);
            }
            networkStream.Write(Encoding.UTF8.GetBytes(res.ToString() + "\n"));
            Console.WriteLine(res.ToString());
        }
        Console.WriteLine("\n\n");
    }

    public void Receive(GpsDataMessage message) => UpdatePosition(message.Value);

    private NetworkStream networkStream;
    public List<AvionicSource> RequestedParameters { get; set; } = [];
    public List<AvionicData> RequestedData { get; set; } = [];
    public TcpClient Client { get; set; }
    public Dictionary<AvionicSource, IBnrConverter> Converters { get; set; } = [];  
    private Dictionary<AvionicSource, double> Values = [];
}
