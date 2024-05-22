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
            // Filter the data based on the threshold configuration
            var filteredData = data.Where(d => d is { } sensorData &&
                sensorData.Temperature > thresholdConfig.TemperatureThreshold &&
                sensorData.LightLevel < thresholdConfig.LightLevelThreshold);
            return Ok(data);
        }
    }
}