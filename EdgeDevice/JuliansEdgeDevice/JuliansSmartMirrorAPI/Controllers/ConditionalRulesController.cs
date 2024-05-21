using Microsoft.AspNetCore.Mvc;
using SmartMirror.Models;

namespace SmartMirror.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConditionalRulesController : ControllerBase
{
    private readonly SmartMirrorContext _context;
    private readonly ArduinoService _arduinoService;

    public ConditionalRulesController(SmartMirrorContext context, ArduinoService arduinoService)
    {
        _context = context;
        _arduinoService = arduinoService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<ConditionalRule>> GetConditionalRules()
    {
        return _context.ConditionalRule;
    }

    [HttpPost]
    public IActionResult UpdateConditionalRule(ConditionalRule rule)
    {
        rule.Id = 1;
        _context.ConditionalRule.Update(rule);
        _context.SaveChanges();
        _arduinoService.SendConditionalRuleToArduino(rule);

        return NoContent();
    }
}