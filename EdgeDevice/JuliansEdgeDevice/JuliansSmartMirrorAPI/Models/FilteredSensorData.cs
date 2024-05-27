namespace SmartMirror.Models
{
    public class FilteredSensorData
    {
        public float? Temperature { get; set; }
        public bool TemperatureAlert { get; set; }
        public float? LightLevel { get; set; }
        public bool LightLevelAlert { get; set; }
        public string? CardUid { get; set; }
        public bool CardAlert { get; set; }
    }
}