using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using AvionicConverter.Converter.BinaryNumberRepresentation;
using AvionicConverter.Converter.Models;
using AvionicConverter.Converter.Models.JsonModel;
using AvionicConverter.STAPServer.Services;

CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
//
// TcpServer server = new TcpServer();
// using var cts = new CancellationTokenSource();
//
// Console.CancelKeyPress += (_, e) =>
// {
//     e.Cancel = true;
//     cts.Cancel();
//     server.Stop();
// };
//
// var thread = new Thread(new ThreadStart(UdpSimClient.StartListener));
// thread.Start();
// await server.StartAsync(cts.Token);
// thread.Interrupt();
// thread.Join();

List<ConverterModel> Models = [
    new BnrConverterModel()
    {
        Name = "GroundSpeedConverter",
        DataBitLength = 15,
        Offset = 5,
        Resolution = 0.125,
        IsSigned = false,
    },
    new BnrConverterModel()
    {
        Name = "TrackConverter",
        DataBitLength = 15,
        Offset = 5,
        Range = 180.0,
        IsSigned = false,
    },
    new BnrConverterCompositeModel()
    {
        Name = "LatitudeConverter",
        Converters = [
            new BnrConverterModel()
            {
                Order = 2,
                IsSigned = false,
                DataBitLength = 11,
                Offset = 9,
                Resolution = 0.000172,
            },
            new BnrConverterModel()
            {
                Order = 1,
                IsSigned = false,
                DataBitLength = 20,
                Offset = 0,
                Resolution = 180.0,
            }
        ]
    }
];

JsonSerializerOptions options = new JsonSerializerOptions()
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
};

var data = JsonSerializer.Serialize(Models, options);
File.WriteAllText("output_Converters.json", data);
