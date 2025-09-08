namespace AvionicConverter.Converter.Models;

public record AvionicSource(ushort? bus = null, ushort? label = null)
{
    public ushort? Bus { get; set; } = bus;
    public ushort? Label { get; set; } = label;

}