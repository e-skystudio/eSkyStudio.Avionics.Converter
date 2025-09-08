using AvionicConverter.Converter.Helpers;
using AvionicConverter.Models;

namespace AvionicConverter.Converter.BinaryNumberRepresentation;

/// <summary>
/// Class for converting BNR values.
/// </summary>
public interface IBnrConverter
{
    public double Decode(AvionicData avionicValue, out BnrStatusMatrix status);
    public AvionicData Encode(double value, BnrStatusMatrix status, AvionicSource? source = null);
}