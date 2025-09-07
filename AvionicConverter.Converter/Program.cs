using AvionicConverter.Converter.BinaryNumberRepresentation;

ulong avionicValue = 0x60FF00;
double resolution = 0.125;
ushort range = 15;
ushort offset = 5;

var converter = new BnrConverterResolution(resolution, range, offset);
double res = converter.Decode(avionicValue, out BnrStatusMatrix status);
Console.WriteLine($"Avionic Value: 0x{avionicValue:X}, Converted Value: {res} - Status: {status}");
Console.WriteLine($"Avionic Value: 0x{converter.Encode(res, BnrStatusMatrix.FailureWarning):X}, Converted Value: {res}");