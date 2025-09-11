using System.Globalization;
using AvionicConverter.Converter.BinaryNumberRepresentation;
using AvionicConverter.Converter.Models;
using AvionicConverter.STAPServer.Services;

CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

TcpServer server = new TcpServer();
using var cts = new CancellationTokenSource();

 //Stop server on Ctrl+C
 Console.CancelKeyPress += (s, e) =>
 {
     e.Cancel = true;
     cts.Cancel();
     server.Stop();
 };

var thread = new Thread(new ThreadStart(UdpSimClient.StartListener));
thread.Start();
await server.StartAsync(cts.Token);
thread.Interrupt();
thread.Join();