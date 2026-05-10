using Siemens.Internship2026.GradeBook.Interfaces;
using Siemens.Internship2026.GradeBook.Models;

namespace Siemens.Internship2026.GradeBook.Repositories;

public class InMemoryGradeRepository : IGradeRepository
{
    protected readonly List<Grade> _items = new();
    protected int _nextId = 1;

    public virtual Task<Grade?> GetByIdAsync(int id)
    {
        var item = _items.FirstOrDefault(i => i.Id == id && i.IsActive);
        return Task.FromResult(item);
    }

    public virtual Task<IEnumerable<Grade>> GetAllAsync()
    {
        var items = _items.Where(i => i.IsActive).AsEnumerable();
        return Task.FromResult(items);
    }
}
