using AvionicConverter.Converter.Helpers;
using AvionicConverter.Converter.Models;
namespace AvionicConverter.Converter.BinaryNumberRepresentation;

/// <summary>
/// Class for converting Binary Number Representation values using a range or resolution.
/// Call factory methods : BnrConverterFromRange or BnrConverterFromResolution to create Binary Number Representation
/// </summary>
public class BnrConverter : IBnrConverter
{
    public static BnrConverter CreateFromRange(double range, ushort dataBitLength, ushort offset, bool isSigned)
    {
        ulong maxValue = isSigned ? (1U << (dataBitLength - 1)) - 1U : (1u << dataBitLength) - 1u;
        double resolution = range / (double)maxValue;
        
        return new BnrConverter(resolution, dataBitLength, offset)
        {
            IsSigned = isSigned,
            MaxValue = maxValue,
            Range = range,
        };
    }

    public static BnrConverter CreateFromResolution(double resolution, ushort dataBitLength, ushort offset)
    {
        return new BnrConverter(resolution, dataBitLength, offset)
        {
            MaxValue = (1u << dataBitLength) - 1u,
        };
    }

    public static BnrConverter CreateEmpty()
    {
        return new BnrConverter();
    }

    public double Resolution { get; set; }
    public ushort DataBitLength { get; set; }
    public ushort Offset { get; set; }
    public bool IsSigned{ get; set; }
    public ulong MaxValue { get; set; }

    public double Decode(AvionicData data, out BnrStatusMatrix status)
    {
        ulong raw = data.AvionicValue.GetBits(DataBitLength, Offset);
        status = (BnrStatusMatrix)data.AvionicValue.GetBits(2, 21);
        return raw * Resolution;
    }

    public (double value, BnrStatusMatrix status) Decode(AvionicData data)
    {
        ulong raw = data.AvionicValue.GetBits(DataBitLength, Offset);
        var status = (BnrStatusMatrix)data.AvionicValue.GetBits(2, 21);
        return (raw * Resolution, status);
    }

    public AvionicData Encode(double value, BnrStatusMatrix status, AvionicSource? source = null)
    {
        ulong raw = (ulong)(value / Resolution) << Offset;
        raw |= (ulong)status << (DataBitLength + Offset + 1);
        var res = new AvionicData(raw) { TimeStamp = DateTime.UtcNow };
        if (source is not null) res.Source = source;
        return res;
    }

    private BnrConverter(double resolution, ushort dataBitLength, ushort offset)
    {
        Resolution = resolution;
        DataBitLength = dataBitLength;
        Offset = offset;
    }

    private BnrConverter()
    {
        Resolution = 1.0;
        DataBitLength = 20;
        Offset = 0;
        IsSigned = false;
        MaxValue = (1u << DataBitLength) - 1u;
    }

    public double? Range { get; set; }
}