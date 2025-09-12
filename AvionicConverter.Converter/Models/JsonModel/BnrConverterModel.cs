using AvionicConverter.Converter.BinaryNumberRepresentation;
using System.Text.Json.Serialization;

namespace AvionicConverter.Converter.Models.JsonModel;

public class AvionicSourceModel
{
    [JsonPropertyName("Bus")]
    public ushort Bus { get; set; }
    [JsonPropertyName("Label")]
    public ushort Label { get; set; }

    public AvionicSource ToAvionicSource() => new(Bus, Label);
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "Type")]
[JsonDerivedType(typeof(BnrConverterModel), typeDiscriminator: "BnrConverter")]
[JsonDerivedType(typeof(BnrConverterCompositeModel), typeDiscriminator: "BnrConverterComposite")]
public class ConverterModel
{
    [JsonPropertyName("Name")] 
    public string? Name { get; set; }
    [JsonPropertyName("AvionicSource")]
    public AvionicSourceModel? AvionicSource { get; set; }
}

public class BnrConverterModel : ConverterModel
{
    [JsonPropertyName("DataBitLength")]
    public ushort DataBitLength { get; set; }
    [JsonPropertyName("Offset")]
    public ushort Offset { get; set; }
    [JsonPropertyName("IsSigned")]
    public bool IsSigned { get; set; } = false;
    [JsonPropertyName("Resolution")]
    public double? Resolution { get; set; }
    [JsonPropertyName("Range")]
    public double? Range { get; set; }
    [JsonPropertyName("Order")]
    public int? Order { get; set; }

    public BnrConverter ToBnrConverter()
    {
        if (Range is not null) return BnrConverter.BnrConverterFromRange(Range.Value, DataBitLength, Offset, IsSigned);
        if (Resolution is not null) return BnrConverter.BnrConverterFromResolution(Resolution.Value, DataBitLength, Offset);
        throw new InvalidOperationException("Either Range or Resolution must be set.");
    }
}
public class BnrConverterCompositeModel : ConverterModel
{
    [JsonPropertyName("Converters")]
    public List<BnrConverterModel> Converters { get; set; } = [];

    public BnrCompositeConverter ToBnrCompositeConverter()
    {
        BnrCompositeConverter composite = new();
        foreach (var converter in Converters)
        {
            var conv = converter.ToBnrConverter();
            composite.AddConverter(converter.AvionicSource?.ToAvionicSource() ?? throw new InvalidOperationException("AvionicSource must be set."), conv, converter.Order ?? 1);
        }
        return composite;
    }
}