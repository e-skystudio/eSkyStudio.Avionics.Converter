using CommunityToolkit.Mvvm.Messaging.Messages;
using System.Text.Json.Serialization;

namespace AvionicConverter.STAPServer.Messages;

public class GpsData()
{
    [JsonPropertyName("Latitude")]
    public double Latitude { get; set; }

    [JsonPropertyName("Longitude")]
    public double Longitude { get; set; }

    [JsonPropertyName("Elevation")]
    public double Elevation { get; set; }

    [JsonPropertyName("Track")]
    public double Track { get; set; }

    [JsonPropertyName("GroundSpeed")]
    public double Groundspeed { get; set; }

    [JsonPropertyName("Heading")]
    public double Heading { get; set; }
}