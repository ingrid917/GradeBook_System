using Siemens.Internship2026.GradeBook.Models;

namespace Siemens.Internship2026.GradeBook.Interfaces;

public interface IItemReader
{
    Task<Grade?> GetByIdAsync(int id);
    Task<IEnumerable<Grade>> GetAllAsync();
}
