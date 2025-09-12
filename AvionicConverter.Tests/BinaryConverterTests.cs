using System.Text.Json;
using System.Threading.Tasks;
using AvionicConverter.Converter.BinaryNumberRepresentation;
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

    [Fact]
    public async Task SavingJsonFile()
    {
        const int expectedCount = 4;
        int res = _service.LoadFromJson(JsonFilePath);
        _service.AddConverter("Altitude", BnrConverter.BnrConverterFromResolution(1.0, 17, 3));
        _service.AddConverter("AltitudeFMGS", BnrConverter.BnrConverterFromResolution(1.0, 16, 4));
        _service.AddConverter("SpeedFMGS", BnrConverter.BnrConverterFromResolution(0.25, 11, 9));
        _service.AddConverter("Mach", BnrConverter.BnrConverterFromResolution(0.0000625, 16, 4));
        _service.AddConverter("SelectedMach", BnrConverter.BnrConverterFromResolution(1.0, 12, 8));
        _service.AddConverter("WindDir", BnrConverter.BnrConverterFromRange(180.0, 12, 8, false));
        _service.AddConverter("WindSpeed", BnrConverter.BnrConverterFromResolution(1.0, 8, 12));
        _service.AddConverter("PitchRoll", BnrConverter.BnrConverterFromResolution(0.01, 14, 6));
        _service.AddConverter("Temperature", BnrConverter.BnrConverterFromResolution(0.25, 11, 9));
        _service.AddConverter("CalibratedAirspeed", BnrConverter.BnrConverterFromResolution(0.0625, 14, 6));
        _service.AddConverter("TrueAirspeed", BnrConverter.BnrConverterFromResolution(0.0625, 15, 5));
        _service.AddConverter("VerticalSpeed", BnrConverter.BnrConverterFromResolution(16.0, 11, 6));
        _service.AddConverter("VerticalSpeedFMGS", BnrConverter.BnrConverterFromResolution(16.0, 10, 10));
        _service.AddConverter("AngleOfAttack", BnrConverter.BnrConverterFromRange(180.0, 12, 8, false));
        _service.AddConverter("Fuel", BnrConverter.BnrConverterFromResolution(40.0, 14, 6));
        await _service.SaveToJson("output.json", false);
        Assert.Equal(expectedCount, res);
    }
}
