using System.Net;
using System.Net.Sockets;
using System.Text;

namespace AvionicConverter.STAPServer.Services;

public class UdpSimClient
{
    private const int listenPort = 49002;

    public static void StartListener()
    {
        UdpClient listener = new UdpClient(listenPort);
        IPEndPoint groupEp = new IPEndPoint(IPAddress.Any, listenPort);

        try
        {
            while (true)
            {
                byte[] bytes = listener.Receive(ref groupEp);
                string text = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                File.AppendAllText("output.txt", $"{text}\n");
                Console.WriteLine(text);
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