using Microsoft.AspNetCore.Mvc;
using SmartMirror.Models;

namespace SmartMirror.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SensorDataController : ControllerBase
{
    private readonly SmartMirrorContext _context;

    public SensorDataController(SmartMirrorContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<IEnumerable<SensorData>> GetSensorData()
    {
        return _context.SensorData.ToList();
    }

    [HttpPost]
    public IActionResult CreateSensorData(SensorData sensorData)
    {
        _context.SensorData.Add(sensorData);
        _context.SaveChanges();

        return CreatedAtAction(nameof(GetSensorData), new { id = sensorData.Id }, sensorData);
    }
}