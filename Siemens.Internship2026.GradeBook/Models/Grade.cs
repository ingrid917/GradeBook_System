using System.Text.Json.Serialization;

namespace Siemens.Internship2026.GradeBook.Models;

public class Grade
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("value")]
    public decimal Value { get; set; }

    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }
}