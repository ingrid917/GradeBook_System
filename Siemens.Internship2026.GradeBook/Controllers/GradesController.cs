using Microsoft.AspNetCore.Mvc;
using Siemens.Internship2026.GradeBook.Interfaces;

namespace Siemens.Internship2026.GradeBook.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GradesController : ControllerBase
{
    private readonly IGradeService _gradeService;
    private readonly IGradeStatisticsService _gradeStatisticsService;
    private readonly ILogger<GradesController> _logger;

    public GradesController(
        IGradeService gradeService,
        IGradeStatisticsService gradeStatisticsService,
        ILogger<GradesController> logger)
    {
        _gradeService = gradeService;
        _gradeStatisticsService = gradeStatisticsService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("GET api/grades called at {Time}", DateTime.UtcNow);

        var grades = await _gradeService.GetAllGradesAsync();
        var response = _gradeStatisticsService.BuildResponse(grades);

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        _logger.LogInformation("GET api/grades/{Id} called at {Time}", id, DateTime.UtcNow);

        try
        {
            var grade = await _gradeService.GetGradeByIdAsync(id);

            if (grade == null)
            {
                return NotFound($"Grade with Id {id} was not found.");
            }

            return Ok(grade);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpGet("passing-active")]
    public async Task<IActionResult> GetFirstPassingActiveGrades([FromQuery] int count)
    {
        _logger.LogInformation(
            "GET api/grades/passing-active called with count {Count} at {Time}",
            count,
            DateTime.UtcNow);

        try
        {
            var grades = await _gradeService.GetFirstPassingActiveGradesAsync(count);

            return Ok(grades);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(exception.Message);
        }
    }
}