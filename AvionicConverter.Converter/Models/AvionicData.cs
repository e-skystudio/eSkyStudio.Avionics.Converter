namespace AvionicConverter.Converter.Models;

public class AvionicData
{
    public AvionicData(ulong value)
    {
        AvionicValue = value;
        TimeStamp = DateTime.UtcNow;
        Source = new AvionicSource();
    }

    public AvionicData()
    {
        TimeStamp = DateTime.UtcNow;
        Source = new AvionicSource();
        AvionicValue = 0x00;
    }
    
    public ulong AvionicValue { get; set; }
    public AvionicSource Source { get; set; }
    public long TimeStampMs
    {
        get => _timestamp;
        set => _timestamp = value;
    }

    public DateTime TimeStamp
    {
        get => DateTimeOffset.FromUnixTimeMilliseconds(_timestamp).DateTime;
        set
        {
            DateTimeOffset dto = DateTime.SpecifyKind(value, DateTimeKind.Unspecified);
            _timestamp = dto.ToUnixTimeMilliseconds();
        }
    }

    private long _timestamp;

    public override string ToString()
    {
        return $"data,{_timestamp},{Source.Bus},{Source.Label},{AvionicValue:X}";
    }
}