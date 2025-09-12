using System.Text.Json;
using AvionicConverter.Converter.Models.JsonModel;
using AvionicConverter.Converter.Services;

namespace AvionicConverter.Tests;

public class BinaryConverterTests
{
    private const string JsonFilePath = "TestData/converters.json";
    private ConverterManagementService _service = new();

    [Fact]
    public void LoadingJsonFile()
    {
        const int expectedCount = 4;
        int res = _service.LoadFromJson(JsonFilePath);
        Assert.Equal(expectedCount, res);
    }
}
