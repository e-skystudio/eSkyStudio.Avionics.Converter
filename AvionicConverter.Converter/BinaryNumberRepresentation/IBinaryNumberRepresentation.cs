using AvionicConverter.Converter.Helpers;

namespace AvionicConverter.Converter.BinaryNumberRepresentation;

/// <summary>
/// Class for converting BNR values.
/// </summary>
public interface IBinaryNumberRepresentation
{
    public double Decode(ulong avionicValue, out BnrStatusMatrix status);
    public ulong Encode(double value, BnrStatusMatrix status);
}