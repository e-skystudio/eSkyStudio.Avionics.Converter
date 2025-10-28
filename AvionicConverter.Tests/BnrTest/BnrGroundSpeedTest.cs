using AvionicConverter.Converter.BinaryNumberRepresentation;
using AvionicConverter.Converter.Models;

namespace AvionicConverter.Tests.BnrTest;

public class BnrGroundSpeedTest
{
    private BnrConverter _converter = BnrConverter.CreateFromResolution(resolution: 0.125, dataBitLength: 15, offset: 5);
    
    [Fact]
    public void DecodeTest()
    {
        const ulong dataIn = 0x60_0F_01; 
        var result = _converter.Decode(new AvionicData(dataIn));
        const double expected = 15.0;
        Assert.Equal(expected, result.value);
    }

    [Theory]
    [InlineData(0x00_0F_00, BnrStatusMatrix.FailureWarning, 15.0)]
    [InlineData(0x20_0F_00, BnrStatusMatrix.NoComputedData, 15.0)]
    [InlineData(0x40_0F_00, BnrStatusMatrix.FunctionalTest, 15.0)]
    [InlineData(0x60_0F_00, BnrStatusMatrix.NormalOps, 15.0)]
    public void Encode(ulong expected, BnrStatusMatrix status, double inValue)
    {
        Assert.Equal(expected, _converter.Encode(inValue, status).AvionicValue);
    }
}