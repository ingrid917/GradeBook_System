using Siemens.Internship2026.GradeBook.Models;

namespace Siemens.Internship2026.GradeBook.Interfaces;

public interface IGradeService
{
    Task<IEnumerable<Grade>> GetAllGradesAsync();

    Task<Grade?> GetGradeByIdAsync(int id);

    Task<IEnumerable<Grade>> GetFirstPassingActiveGradesAsync(int count);
}