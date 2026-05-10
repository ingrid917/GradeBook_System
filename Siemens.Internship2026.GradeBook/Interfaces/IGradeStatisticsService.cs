using Siemens.Internship2026.GradeBook.Models;

namespace Siemens.Internship2026.GradeBook.Interfaces;

public interface IGradeStatisticsService
{
    GradeBookResponse BuildResponse(IEnumerable<Grade> grades);
}