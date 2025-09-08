using System.Globalization;
using AvionicConverter.Converter.BinaryNumberRepresentation;
using AvionicConverter.Models;

CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

// // var gsConverter = new BnrConverterResolution(0.125, 15, 5);
// var gsConverter = BnrConverter.BnrConverterFromResolution(0.125, 15, 5);
// double groundspeed = gsConverter.Decode(0x60FF00UL, out BnrStatusMatrix gsStatus);
// Console.WriteLine($"GroundSpeed: Avionic Value: 0x{0x60FF00UL:X}, Converted Value: {groundspeed} - Status: {gsStatus}");
// Console.WriteLine($"GroundSpeed: Avionic Value: 0x{gsConverter.Encode(groundspeed, BnrStatusMatrix.NormalOps):X}, Converted Value: {groundspeed}");

// var trackConverter  = BnrConverter.BnrConverterFromRange(180.0, 15, 5, false);
// var trackConverter2 = BnrConverter.BnrConverterFromResolution(0.0055, 15, 5);

// double track  = trackConverter.Decode(0x62_15_40UL, out BnrStatusMatrix trackStatus);
// double track2 = trackConverter2.Decode(0x62_15_40UL, out BnrStatusMatrix trackStatus2);
// Console.WriteLine($"Track: Avionic Value: 0x{0x621540UL:X}, Converted Value: {track} - Status: {trackStatus}");
// Console.WriteLine($"Track: Avionic Value: 0x{0x621540UL:X}, Converted Value: {track2} - Status: {trackStatus2}");
// Console.WriteLine($"Track: Avionic Value: 0x{trackConverter.Encode(track, BnrStatusMatrix.NormalOps):X}, Converted Value: {track}");
// Console.WriteLine($"Track: Avionic Value: 0x{trackConverter2.Encode(track2, BnrStatusMatrix.NormalOps):X}, Converted Value: {track2}");

const double expected_latitude = 50.90322271497139;
var latitude = new AvionicData()
{
    AvionicValue = 0x64_86_54,
    Source = new(4, 110),
    TimeStamp = DateTime.UtcNow,
}; 

var latitude_fine = new AvionicData()
{
    AvionicValue = 0x6C_3C_01,
    Source = new(4, 120),
    TimeStamp = DateTime.UtcNow,
};

// var latitudeConverter = BnrConverter.BnrConverterFromRange(180.0, 20, 0, false);
// var latitudeFineConverter = BnrConverter.BnrConverterFromRange(0.000172, 11, 9, false);

var composite_converter = new BnrCompositeConverter();
composite_converter.AddConverter(new(4, 120), BnrConverter.BnrConverterFromRange(0.000172, 11, 9, false), 2);
composite_converter.AddConverter(new(4, 110), BnrConverter.BnrConverterFromRange(180.0, 20, 0, false), 1);

BnrStatusMatrix sts = BnrStatusMatrix.FailureWarning;
double oLat = double.MinValue;
oLat = composite_converter.Decode(latitude, out sts);
oLat = composite_converter.Decode(latitude_fine, out sts);

Console.WriteLine($"\n\n****EXCPECTED OUTPUT LATITUDE : {expected_latitude} ****");
Console.WriteLine($"****          OUTPUT LATITUDE : {oLat} ****");


AvionicData oLatData = composite_converter.Encode(oLat, BnrStatusMatrix.NormalOps, latitude.Source);
AvionicData oLatFineData = composite_converter.Encode(oLat, BnrStatusMatrix.NormalOps, latitude_fine.Source);

Console.WriteLine("\n\n");
Console.WriteLine($"****EXCPECTED OUTPUT LATITUDE     : {latitude.AvionicValue:X} ****");
Console.WriteLine($"****          OUTPUT LATITUDE     : {oLatData.AvionicValue:X} ****");
Console.WriteLine($"****EXCPECTED OUTPUT LATITUDE FINE: {latitude_fine.AvionicValue:X} ****");
Console.WriteLine($"****          OUTPUT LATITUDE FINE: {oLatFineData.AvionicValue:X} ****");