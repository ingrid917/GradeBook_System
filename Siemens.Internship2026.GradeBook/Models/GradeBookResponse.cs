namespace Siemens.Internship2026.GradeBook.Models;

public class GradeBookResponse
{
    public IEnumerable<Grade> Data { get; set; } = new List<Grade>();

    public GradeStatistics Statistics { get; set; } = new();
}