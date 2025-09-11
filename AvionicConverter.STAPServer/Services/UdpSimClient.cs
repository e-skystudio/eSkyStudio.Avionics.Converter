using AvionicConverter.STAPServer.Messages;
using CommunityToolkit.Mvvm.Messaging;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace AvionicConverter.STAPServer.Services;

public class UdpSimClient
{
    private const int listenPort = 50_555;

    public static void ParseGpsData(string datas)
    {
        var gpsData = JsonSerializer.Deserialize<GpsData>(datas, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        if(gpsData == null) return;
        WeakReferenceMessenger.Default.Send(new GpsDataMessage(gpsData));
    }

    public static void StartListener()
    {
        UdpClient listener = new UdpClient(listenPort);
        IPEndPoint groupEp = new IPEndPoint(IPAddress.Any, listenPort);
        if(File.Exists("output.txt")) File.Delete("output.txt");
        try
        {
            while (true)
            {
                byte[] bytes = listener.Receive(ref groupEp);

                string text = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                ParseGpsData(text);
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            listener.Close();
        }
    }
}