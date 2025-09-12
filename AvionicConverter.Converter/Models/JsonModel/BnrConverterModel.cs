using System.Text.Json.Serialization;

namespace AvionicConverter.Converter.Models.JsonModel;

public class AvionicConverterModel
{
    [JsonPropertyName("Bus")]
    public ushort Bus { get; set; }
    [JsonPropertyName("Label")]
    public ushort Label { get; set; }
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "Type")]
[JsonDerivedType(typeof(BnrConverterModel), typeDiscriminator: "BnrConverter")]
[JsonDerivedType(typeof(BnrConverterCompositeModel), typeDiscriminator: "BnrConverterComposite")]
public class ConverterModel
{
    [JsonPropertyName("Name")] 
    public string? Name { get; set; }
}

public class BnrConverterModel : ConverterModel
{
    [JsonPropertyName("DataBitLength")]
    public int DataBitLength { get; set; }
    [JsonPropertyName("Offset")]
    public int Offset { get; set; }
    [JsonPropertyName("IsSigned")]
    public bool IsSigned { get; set; }
    [JsonPropertyName("Resolution")]
    public double? Resolution { get; set; }
    [JsonPropertyName("Range")]
    public double? Range { get; set; }
    [JsonPropertyName("Order")]
    public int? Order { get; set; }
}
public class BnrConverterCompositeModel : ConverterModel
{
    [JsonPropertyName("Converters")]
    public List<BnrConverterModel> Converters { get; set; } = [];
}