using AvionicConverter.Converter.Helpers;

namespace AvionicConverter.Converter.BinaryNumberRepresentation;

/// <summary>
/// Class for converting BNR values.
/// </summary>
public class BnrConverterResolution(double resoltion, ushort range, ushort offset)
{
    public double Resolution { get; set; } = resoltion;
    public ushort Range { get; set; } = range;
    public ushort Offset { get; set; } = offset;

    public double Decode(ulong avionicValue, out BnrStatusMatrix status)
    {
        ulong raw = avionicValue.GetBits(Range, Offset);
        status = (BnrStatusMatrix)avionicValue.GetBits(2, 21);

        return (double)raw * Resolution;
    }

    public ulong Encode(double value, BnrStatusMatrix status)
    {
        ulong raw = (ulong)(value / Resolution) << Offset;
        raw |= (ulong)status << 21;
        return raw;
    }
}