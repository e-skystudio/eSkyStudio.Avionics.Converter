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

        Converters.Add(new(4, 110), latitudeConverter);
        Converters.Add(new(4, 111), longitudeConverter);
        Converters.Add(new(4, 120), latitudeConverter);
        Converters.Add(new(4, 121), longitudeConverter);
        Converters.Add(new(4, 112), gsConverter);
        Converters.Add(new(4, 103), trackConverter);
        Converters.Add(new(2, 314), trackConverter);

        Values = new Dictionary<AvionicSource, double>
        {
            { new AvionicSource(4, 103), _track },
            { new AvionicSource(4, 110), _latitude },
            { new AvionicSource(4, 111), _longitude },
            { new AvionicSource(4, 112), _groundSpeed },
            { new AvionicSource(4, 120), _latitude },
            { new AvionicSource(4, 121), _longitude },
            { new AvionicSource(2, 314), _heading }
        };
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
        if (!_timer.Enabled) _timer.Start();

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


    public void UpdatePosition()
    {
        _groundSpeed++;
        _groundSpeed %= 4096;
        _track++;
        _track %= 360.0;
        Values[new(4, 112)] = _groundSpeed;
        Values[new(4, 103)] = _track;
        Console.WriteLine($"Aircraft Position : {_latitude} - {_longitude} - Speed: {_groundSpeed} - Track :{_track}");
    }

    private void SendDataCallback(object? sender, ElapsedEventArgs e)
    {
        UpdatePosition();
        foreach (AvionicSource source in RequestedParameters)
        {
            var res = Converters[source].Encode(Values[source], BnrStatusMatrix.NormalOps, source);
            networkStream.Write(Encoding.UTF8.GetBytes(res.ToString() + "\n"));
            Console.WriteLine(res.ToString());
        }
        Console.WriteLine("\n\n");
    }
    private NetworkStream networkStream;
    public List<AvionicSource> RequestedParameters { get; set; } = [];
    public List<AvionicData> RequestedData { get; set; } = [];
    private readonly Timer _timer;
    public TcpClient Client { get; set; }
    public Dictionary<AvionicSource, IBnrConverter> Converters { get; set; } = [];  
    private double _latitude = 51.11861111;
    private double _longitude = 4.84222222;
    private double _groundSpeed = 100.0;
    private double _track = 1.0;
    private double _heading = 0.0;

    private Dictionary<AvionicSource, double> Values = [];
}
