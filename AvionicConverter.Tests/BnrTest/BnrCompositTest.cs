using AvionicConverter.Converter.BinaryNumberRepresentation;
using AvionicConverter.Converter.Models;

namespace AvionicConverter.Tests.BnrTest;

public class BnrCompositeTest
{
    private readonly BnrCompositeConverter _latitudeConverter;
    private readonly BnrCompositeConverter _longitudeConverter;
    private const double VorLatitude = 51.11861111; //BUN (EB)  VOR LATITUDE
    private const double VorLongitude = 4.84222222; //BUN (EB) VOR LONGITUDE
    
    public BnrCompositeTest()
    {
        _latitudeConverter = new(
            (new AvionicSource(bus: 4, label: 110), BnrConverter.CreateFromRange(range: 180.0, dataBitLength: 20, offset: 0, isSigned: true)), 
            (new AvionicSource(bus: 4, label: 120), BnrConverter.CreateFromRange(range: 0.000172, dataBitLength: 11, offset: 9, isSigned: true)));
        
        _longitudeConverter = new(
            (new AvionicSource(bus: 4, label: 111), BnrConverter.CreateFromRange(range: 180.0, dataBitLength: 20, offset: 0, isSigned: true)), 
            (new AvionicSource(bus: 4, label: 121), BnrConverter.CreateFromRange(range: 0.000172, dataBitLength: 11, offset: 9, isSigned: true)));
    }

    [Fact]
    public void EncodeLatitudeStep1()
    {
        AvionicSource latitudeSource = new(4, 110);
        var res = _latitudeConverter.Encode(VorLatitude, BnrStatusMatrix.NormalOps, latitudeSource);
        const ulong expected = 0x648B3B;        
        Assert.Equal(expected, res.AvionicValue);
    }

    [Fact]
    public void EncodeLatitudeStep2()
    {
        AvionicSource latitudeFineSource = new(4, 120);    
        var res = _latitudeConverter.Encode(VorLatitude, BnrStatusMatrix.NormalOps, latitudeFineSource);
        const ulong expected = 0x67E401;
    }
    
    [Fact]                                                                                              
    public void EncodeLongitudeStep1()
    {
        AvionicSource longitudeSource = new(4, 111);
        var res = _longitudeConverter.Encode(VorLongitude, BnrStatusMatrix.NormalOps, longitudeSource);       
        const ulong expected = 0x606E2F;                                                                
        Assert.Equal(expected, res.AvionicValue);                                                       
    }                                                                                                   
                                                                                                     
    [Fact]                                                                                              
    public void EncodeLongitudeStep2()                                                                   
    {                 
        AvionicSource longitudeFineSource = new(4, 121);
        var res = _longitudeConverter.Encode(VorLongitude, BnrStatusMatrix.NormalOps, longitudeFineSource);       
        const ulong expected = 0x6FD201;                                                                
    }                                                                                                   
}