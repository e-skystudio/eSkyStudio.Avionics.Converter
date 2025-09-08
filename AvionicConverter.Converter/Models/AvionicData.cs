namespace AvionicConverter.Converter.Models;

public class AvionicData
{
    public ulong AvionicValue { get; set; }
    public AvionicSource Source { get; set; } = new AvionicSource();
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
}