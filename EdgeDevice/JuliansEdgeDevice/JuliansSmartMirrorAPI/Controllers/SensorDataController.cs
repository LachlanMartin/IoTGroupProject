using Microsoft.AspNetCore.Mvc;
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
            var data = _mqttService.GetFilteredData();
            return Ok(data);
        }
    }
}