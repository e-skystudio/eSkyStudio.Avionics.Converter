using System.Text.Json;
using AvionicConverter.Converter.Models.JsonModel;

namespace AvionicConverter.Tests;

public class BinaryConverterTests
{
    [Fact]
    public void LoadingJsonFile()
    {
        string data = File.ReadAllText("Resources/Converters.json"); 
        var converters = JsonSerializer.Deserialize<List<ConverterModel>>(data);
        Assert.NotNull(converters);
        Assert.Equal(2, converters.Count);
    }
}
