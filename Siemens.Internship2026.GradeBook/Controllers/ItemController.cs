using Microsoft.AspNetCore.Mvc;
using Siemens.Internship2026.GradeBook.Interfaces;

namespace Siemens.Internship2026.GradeBook.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemController : ControllerBase
{
    private readonly IItemReader _reader;

    public ItemController(IItemReader reader)
    {
        _reader = reader;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        Console.WriteLine($"[LOG] {DateTime.UtcNow}: GET api/item called");

        var items = await _reader.GetAllAsync();
        var itemList = items.ToList();

        var totalCount = itemList.Count;
        var averageValue = itemList.Any() ? itemList.Average(i => i.Value) : 0;

        Console.WriteLine($"[LOG] Returning {totalCount} items, average value: {averageValue}");

        return Ok(new
        {
            Data = itemList,
            Statistics = new
            {
                TotalCount = totalCount,
                AverageValue = averageValue,
                RetrievedAt = DateTime.UtcNow
            }
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        Console.WriteLine($"[LOG] {DateTime.UtcNow}: GET api/item/{id} called");

        if (id <= 0)
        {
            Console.WriteLine($"[LOG] Invalid id: {id}");
            return BadRequest("Id must be a positive integer.");
        }

        var item = await _reader.GetByIdAsync(id);
        if (item == null)
        {
            Console.WriteLine($"[LOG] Item {id} not found");
            return NotFound($"Item with Id {id} was not found.");
        }

        return Ok(item);
    }
}
