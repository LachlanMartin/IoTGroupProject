namespace SmartMirror.Models;

public class ConditionalRule
{
    public int Id { get; set; }
    public double UpdateInterval { get; set; }
    public float TemperatureThreshold { get; set; }
    public bool MotionEnabled { get; set; }
}