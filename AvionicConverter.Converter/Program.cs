using System.Globalization;
using AvionicConverter.Converter.BinaryNumberRepresentation;

CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

// var gsConverter = new BnrConverterResolution(0.125, 15, 5);
var gsConverter = BnrConverter.BnrConverterFromResolution(0.125, 15, 5);
double groundspeed = gsConverter.Decode(0x60FF00UL, out BnrStatusMatrix gsStatus);
Console.WriteLine($"GroundSpeed: Avionic Value: 0x{0x60FF00UL:X}, Converted Value: {groundspeed} - Status: {gsStatus}");
Console.WriteLine($"GroundSpeed: Avionic Value: 0x{gsConverter.Encode(groundspeed, BnrStatusMatrix.NormalOps):X}, Converted Value: {groundspeed}");

var trackConverter  = BnrConverter.BnrConverterFromRange(180.0, 15, 5, false);
var trackConverter2 = BnrConverter.BnrConverterFromResolution(0.0055, 15, 5);

double track  = trackConverter.Decode(0x62_15_40UL, out BnrStatusMatrix trackStatus);
double track2 = trackConverter2.Decode(0x62_15_40UL, out BnrStatusMatrix trackStatus2);
Console.WriteLine($"Track: Avionic Value: 0x{0x621540UL:X}, Converted Value: {track} - Status: {trackStatus}");
Console.WriteLine($"Track: Avionic Value: 0x{0x621540UL:X}, Converted Value: {track2} - Status: {trackStatus2}");
Console.WriteLine($"Track: Avionic Value: 0x{trackConverter.Encode(track, BnrStatusMatrix.NormalOps):X}, Converted Value: {track}");
Console.WriteLine($"Track: Avionic Value: 0x{trackConverter2.Encode(track2, BnrStatusMatrix.NormalOps):X}, Converted Value: {track2}");