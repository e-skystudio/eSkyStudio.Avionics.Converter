
namespace AvionicConverter.Converter.Helpers;

/// <summary>
/// Class for converting BNR values.
/// </summary>
public static class UnsignedLongHelpers
{
    public static ulong GetBits(this ulong value, int length, int offset=0) => (value >> offset) & ((1UL << length) - 1UL);

    public static ulong ClearBits(this ulong value, int length, int offset=0) => value & ~(((1UL << length) - 1UL) << offset);
}