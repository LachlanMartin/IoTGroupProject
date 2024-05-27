using Microsoft.AspNetCore.Mvc;
using SmartMirror.Models;
using SmartMirror.Services;

namespace SmartMirror.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SensorDataController : ControllerBase
    {
        private readonly MqttService _mqttService;

        public SensorDataController(MqttService mqttService)
        {
            _mqttService = mqttService;
        }

        [HttpGet("filtered")]
        public IActionResult GetFilteredSensorData()
        {
            var data = _mqttService.GetData();
            var thresholdConfig = _mqttService.GetThresholdConfig();
            var now = DateTime.UtcNow;

            var temperatureData = data.FirstOrDefault(d => d?.SensorData?.Temperature > thresholdConfig?.TemperatureThreshold);
            var lightLevelData = data.FirstOrDefault(d => d?.SensorData?.LightLevel < thresholdConfig?.LightLevelThreshold);
            var cardData = data.FirstOrDefault(d => d?.CardData?.Timestamp > now.AddSeconds(-5));

            var result = new FilteredSensorData
            {
                Temperature = data?.FirstOrDefault()?.SensorData?.Temperature,
                TemperatureAlert = temperatureData != null,
                LightLevel = data?.FirstOrDefault()?.SensorData?.LightLevel,
                LightLevelAlert = lightLevelData != null,
                CardUid = cardData?.CardData?.CardUid ?? "Not scanned",
                CardAlert = cardData != null
            };

            return Ok(result);
        }

        [HttpGet("thresholds")]
        public IActionResult GetThresholds()
        {
            var thresholdConfig = _mqttService.GetThresholdConfig();
            return Ok(thresholdConfig);
        }
    }
}