namespace SmartMirror.Models;

public class SensorData
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    public float Temperature { get; set; }
    public int LightLevel { get; set; }
}