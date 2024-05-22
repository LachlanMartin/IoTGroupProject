namespace SmartMirror.Models;

public class SensorData
{
    public DateTime Timestamp { get; set; }
    public float Temperature { get; set; }
    public float LightLevel { get; set; }
}