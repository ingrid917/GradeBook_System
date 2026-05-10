using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using Siemens.Internship2026.GradeBook.Configuration;
using Siemens.Internship2026.GradeBook.Interfaces;
using Siemens.Internship2026.GradeBook.Models;

namespace Siemens.Internship2026.GradeBook.Repositories;

public class HttpGradeRepository : IGradeRepository
{
    private readonly HttpClient _httpClient;
    private readonly ExternalGradeSourceOptions _options;
    private readonly ILogger<HttpGradeRepository> _logger;

    public HttpGradeRepository(
        HttpClient httpClient,
        IOptions<ExternalGradeSourceOptions> options,
        ILogger<HttpGradeRepository> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<IEnumerable<Grade>> GetAllAsync()
    {
        if (string.IsNullOrWhiteSpace(_options.Url))
        {
            throw new InvalidOperationException("External grade source URL is not configured.");
        }

        try
        {
            var grades = await _httpClient.GetFromJsonAsync<List<Grade>>(_options.Url);

            return grades ?? new List<Grade>();
        }
        catch (HttpRequestException exception)
        {
            _logger.LogError(exception, "Failed to fetch grades from external endpoint.");

            throw new InvalidOperationException("Could not fetch grades from the external source.", exception);
        }
    }

    public async Task<Grade?> GetByIdAsync(int id)
    {
        var grades = await GetAllAsync();

        return grades.FirstOrDefault(grade => grade.Id == id);
    }
}