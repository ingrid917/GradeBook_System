using Microsoft.AspNetCore.Mvc;
using Siemens.Internship2026.GradeBook.Interfaces;

namespace Siemens.Internship2026.GradeBook.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GradesController : ControllerBase
{
    private readonly IGradeRepository _gradeRepository;
    private readonly IGradeStatisticsService _gradeStatisticsService;
    private readonly ILogger<GradesController> _logger;

    public GradesController(
        IGradeRepository gradeRepository,
        IGradeStatisticsService gradeStatisticsService,
        ILogger<GradesController> logger)
    {
        _gradeRepository = gradeRepository;
        _gradeStatisticsService = gradeStatisticsService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("GET api/grades called at {Time}", DateTime.UtcNow);

        var grades = await _gradeRepository.GetAllAsync();
        var response = _gradeStatisticsService.BuildResponse(grades);

        _logger.LogInformation(
            "Returning {TotalCount} grades with average value {AverageValue}",
            response.Statistics.TotalCount,
            response.Statistics.AverageValue);

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        _logger.LogInformation("GET api/grades/{Id} called at {Time}", id, DateTime.UtcNow);

        if (id <= 0)
        {
            _logger.LogWarning("Invalid grade id: {Id}", id);

            return BadRequest("Id must be a positive integer.");
        }

        var grade = await _gradeRepository.GetByIdAsync(id);

        if (grade == null)
        {
            _logger.LogWarning("Grade with id {Id} was not found.", id);

            return NotFound($"Grade with Id {id} was not found.");
        }

        return Ok(grade);
    }
}