using Siemens.Internship2026.GradeBook.Interfaces;
using Siemens.Internship2026.GradeBook.Models;

namespace Siemens.Internship2026.GradeBook.Services;

public class GradeService : IGradeService
{
    private const decimal PassingGradeValue = 5m;

    private readonly IGradeRepository _gradeRepository;

    public GradeService(IGradeRepository gradeRepository)
    {
        _gradeRepository = gradeRepository;
    }

    public async Task<IEnumerable<Grade>> GetAllGradesAsync()
    {
        return await _gradeRepository.GetAllAsync();
    }

    public async Task<Grade?> GetGradeByIdAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Id must be a positive integer.", nameof(id));
        }

        return await _gradeRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Grade>> GetFirstPassingActiveGradesAsync(int count)
    {
        if (count <= 0)
        {
            throw new ArgumentException("Count must be a positive integer.", nameof(count));
        }

        var grades = await _gradeRepository.GetAllAsync();

        return grades
            .Where(grade => grade.IsActive)
            .Where(grade => grade.Value >= PassingGradeValue)
            .Take(count)
            .ToList();
    }
}