using AvionicConverter.Converter.Helpers;

namespace AvionicConverter.Converter.BinaryNumberRepresentation;

/// <summary>
/// Class for converting Binary Number Representation values using a range or resolution.
/// Call factory methods : BnrConverterFromRange or BnrConverterFromResolution to create Binary Number Representation
/// </summary>
public class BnrConverter : IBinaryNumberRepresentation
{
    public static BnrConverter BnrConverterFromRange(double range, ushort dataBitLength, ushort offset, bool isSigned)
    {
        ulong maxValue = isSigned ? (1U << (dataBitLength - 1)) - 1U : (1u << dataBitLength) - 1u;
        double resolution = range / (double)maxValue;
        return new BnrConverter(resolution, dataBitLength, offset);
    }

    public static BnrConverter BnrConverterFromResolution(double resoltion, ushort dataBitLength, ushort offset)
    {
        return new BnrConverter(resoltion, dataBitLength, offset);
    }

    public double Resolution { get; set; }
    public ushort DataBitLength { get; set; }
    public ushort Offset { get; set; }

    public double Decode(ulong avionicValue, out BnrStatusMatrix status)
    {
        ulong raw = avionicValue.GetBits(DataBitLength, Offset);
        status = (BnrStatusMatrix)avionicValue.GetBits(2, 21);
        return raw * Resolution;
    }

    public ulong Encode(double value, BnrStatusMatrix status)
    {
        ulong raw = (ulong)(value / Resolution) << Offset;
        raw |= (ulong)status << 21;
        return raw;
    }

    private BnrConverter(double resoltion, ushort dataBitLength, ushort offset)
    {
        Resolution = resoltion;
        DataBitLength = dataBitLength;
        Offset = offset;
    }
}