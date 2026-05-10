using Siemens.Internship2026.GradeBook.Interfaces;
using Siemens.Internship2026.GradeBook.Models;

namespace Siemens.Internship2026.GradeBook.Services;

public class GradeStatisticsService : IGradeStatisticsService
{
    public GradeBookResponse BuildResponse(IEnumerable<Grade> grades)
    {
        var gradeList = grades.ToList();

        var statistics = new GradeStatistics
        {
            TotalCount = gradeList.Count,
            AverageValue = gradeList.Any() ? gradeList.Average(grade => grade.Value) : 0,
            RetrievedAt = DateTime.UtcNow
        };

        return new GradeBookResponse
        {
            Data = gradeList,
            Statistics = statistics
        };
    }
}