using AvionicConverter.Converter.Helpers;
using AvionicConverter.Converter.Models;

namespace AvionicConverter.Converter.BinaryNumberRepresentation;

public class BnrCompositeConverter : IBnrConverter
{
    public void AddConverter(AvionicSource source, BnrConverter converter, int order)
    {
        OrderedSource[order] = source; 
        _converters.Add(source, converter);
        _values.Add(source, 0);
        Resolution = _converters[OrderedSource.Last().Value].Resolution;
    }


    public double Decode(AvionicData avionicValue, out BnrStatusMatrix status)
    {
        _values[avionicValue.Source] = _converters[avionicValue.Source].Decode(avionicValue, out status);
        return _values.Sum(v => v.Value);
    }

    public AvionicData Encode(double value, BnrStatusMatrix status, AvionicSource? source = null)
    {
        ulong scaledValue = (ulong)Math.Round(value / Resolution);
        if (source is null || !_converters.ContainsKey(source))
        {
            throw new ArgumentException("Source is null or no converter binded to source !");
        }
        var converter = _converters[source];
        int bitOffset = GetHigherOrderBitsTotal(source);
        ulong avionicValue = scaledValue.GetBits(converter.DataBitLength, bitOffset);
        avionicValue <<= converter.Offset;
        if (converter.IsSigned && value < 0.0)
            avionicValue |= 1UL << (converter.Offset + converter.DataBitLength);
        if (converter.DataBitLength < 20)
            avionicValue |= 1UL;
        // if (source.Signed && value.Value < 0)
        //     avionicValue |= 1UL << source.DataEndBit;

        // if ((source.DataEndBit - source.DataStartBit) < 20)
        //     avionicValue |= 1UL;

        avionicValue |= (ulong)status << (converter.Offset + converter.DataBitLength + 1);

        // return new AvionicData(bus: bus, label: label, avionicValue);
        return new AvionicData(){ AvionicValue = avionicValue, Source = source, TimeStamp = DateTime.UtcNow};
    }

    private int GetHigherOrderBitsTotal(AvionicSource source)
    {
        int offset = 0;
        int index = OrderedSource.IndexOfValue(source);
        if (index == -1)
            return 0; // object not found

        int currentKey = OrderedSource.Keys[index];

        // Enumerate everything with greater key
        foreach (var kvp in OrderedSource)
        {
            if (kvp.Key > currentKey)
                offset += _converters[kvp.Value].DataBitLength;
        }
        return offset;
    }

    public SortedList<int, AvionicSource> OrderedSource { get; set; } = [];
    public BnrConverter? GetConverter(AvionicSource source) => _converters[source];

    private Dictionary<AvionicSource, BnrConverter> _converters = [];
    private Dictionary<AvionicSource, double> _values = [];

    public double Resolution { get; set; }
}