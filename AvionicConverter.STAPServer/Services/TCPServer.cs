using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;

namespace AvionicConverter.STAPServer.Services;

public class TcpServer
{
    private readonly int _port;
    private TcpListener? _listener;
    private List<TcpClient> _clients = [];


    public TcpServer(int port = 8766)
    {
        _port = port;
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        _listener = new TcpListener(IPAddress.Any, _port);
        _listener.Start();
        Console.WriteLine($"[Server] Listening on port {_port}...");

        while (!cancellationToken.IsCancellationRequested)
        {
            _clients.Add(await _listener.AcceptTcpClientAsync(cancellationToken));
            var client = new StapClient(_clients.Last());
            _ = client.HandleClientAsync(); // fire & forget
        }
    }




    
    public void Stop()
    {
        _listener?.Stop();
        Console.WriteLine("[Server] Stopped.");
    }
}