// using Microsoft.AspNetCore.Mvc;

// [ApiController]
// [Route("api/[controller]")]
// public class SettingsController : ControllerBase
// {
//     private readonly SmartMirrorContext _context;

//     public SettingsController(SmartMirrorContext context)
//     {
//         _context = context;
//     }

//     [HttpGet]
//     public ActionResult<IEnumerable<SensorData>> Update()
//     {
//         return _context.SensorData.ToList();
//     }

//     [HttpPost]
//     public IActionResult CreateSensorData(SensorData sensorData)
//     {
//         _context.SensorData.Add(sensorData);
//         _context.SaveChanges();

//         return CreatedAtAction(nameof(GetSensorData), new { id = sensorData.Id }, sensorData);
//     }
// }