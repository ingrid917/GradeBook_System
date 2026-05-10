# GradeBook_System

# SOLID Review and Refactoring Report

This document describes the code review and refactoring process for the `Siemens.Internship2026.GradeBook` project.

## Step 1: Add `.gitignore` and Remove Generated Files

Before changing the application code, I first cleaned the repository structure.

The project should not include generated files or local IDE configuration files, such as:

```text
bin/
obj/
.vs/
*.user
```

```gitignore
# Build results
bin/
obj/

# Visual Studio
.vs/
*.user
*.suo

# Rider / JetBrains
.idea/

# OS files
.DS_Store
Thumbs.db
```

This keeps the repository cleaner and easier to use on another machine.

### Related commit

```text
Add gitignore and remove generated files
```

---

## Step 2: Rename `Item` Model to `Grade`

The original model was named `Item`.

This name was too generic for a project called `GradeBook`. A class name should describe the domain concept it represents. Since the application works with grade book data, the model was renamed from `Item` to `Grade`.

### Original problem

```text
Models/Item.cs
```

The name `Item` did not clearly explain what the object represented.

### Applied change

The original model was replaced with:

```text
Models/Grade.cs
```

```csharp
namespace Siemens.Internship2026.GradeBook.Models;

public class Grade
{
    public int Id { get; set; }

    public decimal Value { get; set; }

    public bool IsActive { get; set; } = true;
}
```

### Why this change was made

This change improves readability. When someone reads the code, `Grade` is much easier to understand than `Item`.

This is not a direct SOLID violation, but it is an important clean code improvement because meaningful names make the project easier to maintain.

### Related commit

```text
Rename Item model to Grade
```



# 2. Renaming `IItemReader` to `IGradeRepository`

## Original problem

The original interface was named:

```csharp
IItemReader
```

It was responsible for reading data, but the name did not clearly describe the domain.

Also, in common layered architecture, data access is usually represented using a repository.

## Applied change

The file:

```text
Interfaces/IItemReader.cs
```

was replaced with:

```text
Interfaces/IGradeRepository.cs
```

## Refactored code

```csharp
using Siemens.Internship2026.GradeBook.Models;

namespace Siemens.Internship2026.GradeBook.Interfaces;

public interface IGradeRepository
{
    Task<Grade?> GetByIdAsync(int id);

    Task<IEnumerable<Grade>> GetAllAsync();
}
```

## SOLID principle involved

This supports the **Dependency Inversion Principle**.

The controller does not depend directly on a concrete repository class. Instead, it depends on the abstraction `IGradeRepository`.

## Why this is better

The interface now clearly states that it is responsible for accessing grade data.

It also allows the implementation to be changed later. For example, the application could replace the in-memory repository with a database repository without changing the controller logic.


# 3. Renaming `ItemRepository` to `InMemoryGradeRepository`

## Original problem

The original repository was called:

```csharp
ItemRepository
```

This name had two issues:

1. `Item` was too generic;
2. the name did not explain where the data was stored.

The original code also used `protected` members and `virtual` methods although inheritance was not needed.

## Applied change

The file:

```text
Repositories/ItemRepository.cs
```

was replaced with:

```text
Repositories/InMemoryGradeRepository.cs
```

## Refactored code

```csharp
using Siemens.Internship2026.GradeBook.Interfaces;
using Siemens.Internship2026.GradeBook.Models;

namespace Siemens.Internship2026.GradeBook.Repositories;

public class InMemoryGradeRepository : IGradeRepository
{
    private readonly List<Grade> _grades = new()
    {
        new Grade { Id = 1, Value = 9.50m },
        new Grade { Id = 2, Value = 8.75m },
        new Grade { Id = 3, Value = 10.00m },
        new Grade { Id = 4, Value = 7.25m, IsActive = false }
    };

    public Task<Grade?> GetByIdAsync(int id)
    {
        var grade = _grades.FirstOrDefault(grade => grade.Id == id && grade.IsActive);

        return Task.FromResult(grade);
    }

    public Task<IEnumerable<Grade>> GetAllAsync()
    {
        var grades = _grades
            .Where(grade => grade.IsActive)
            .AsEnumerable();

        return Task.FromResult(grades);
    }
}
```

## SOLID principle involved

This supports the **Single Responsibility Principle**.

The repository has one clear responsibility:

```text
Provide access to grade data stored in memory.
```

## Why this is better

The name `InMemoryGradeRepository` explains exactly what the class does:

- it works with grades;
- it stores data in memory;
- it is a repository.

The class no longer exposes unnecessary protected members or virtual methods.

---


# 4. Adding Explicit Response Models

## Original problem

The original controller created the API response using an anonymous object:

```csharp
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
```

This works, but it makes the API response structure less explicit.

If the response becomes more complex, anonymous objects can make the code harder to understand, test, and document.

## Applied change

Two explicit response models were added:

```text
Models/GradeStatistics.cs
Models/GradeBookResponse.cs
```

## Refactored code: `GradeStatistics.cs`

```csharp
namespace Siemens.Internship2026.GradeBook.Models;

public class GradeStatistics
{
    public int TotalCount { get; set; }

    public decimal AverageValue { get; set; }

    public DateTime RetrievedAt { get; set; }
}
```

## Refactored code: `GradeBookResponse.cs`

```csharp
namespace Siemens.Internship2026.GradeBook.Models;

public class GradeBookResponse
{
    public IEnumerable<Grade> Data { get; set; } = new List<Grade>();

    public GradeStatistics Statistics { get; set; } = new();
}
```

## SOLID principle involved

This mainly improves maintainability and supports the **Single Responsibility Principle**.

The response structure is now represented by model classes instead of being constructed directly inside the controller.

## Why this is better

The API response is now explicit:

```text
GradeBookResponse
 ├── Data
 └── Statistics
```

This makes the code easier to read and easier to extend.

---


# 5. Moving Statistics Calculation to a Service

## Original problem

In the original controller, the `GetAll()` method calculated statistics directly:

```csharp
var items = await _reader.GetAllAsync();
var itemList = items.ToList();

var totalCount = itemList.Count;
var averageValue = itemList.Any() ? itemList.Average(i => i.Value) : 0;
```

This means the controller was responsible for:

- receiving the HTTP request;
- reading data;
- calculating statistics;
- building the response;
- logging information.

This is too much for one controller method.

## SOLID principle violated

This violates the **Single Responsibility Principle**.

A controller should mainly handle HTTP requests and responses. Business logic, such as calculating statistics, should be moved to a service.

## Applied change

A new service interface and implementation were added:

```text
Interfaces/IGradeStatisticsService.cs
Services/GradeStatisticsService.cs
```

## Refactored code: `IGradeStatisticsService.cs`

```csharp
using Siemens.Internship2026.GradeBook.Models;

namespace Siemens.Internship2026.GradeBook.Interfaces;

public interface IGradeStatisticsService
{
    GradeBookResponse BuildResponse(IEnumerable<Grade> grades);
}
```

## Refactored code: `GradeStatisticsService.cs`

```csharp
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
```

## Why this is better

The controller no longer calculates statistics.

Before:

```text
Controller calculates statistics.
```

After:

```text
GradeStatisticsService calculates statistics.
```

This makes the controller smaller and easier to maintain.

---


# 6. Replacing `ItemController` with `GradesController`

## Original problem

The original controller was named:

```csharp
ItemController
```

It used the route:

```text
api/item
```

This did not match the domain of the application. Since the application is a GradeBook, the controller should represent grades.

The original controller also contained too much logic and used `Console.WriteLine` for logging.

## Applied change

The file:

```text
Controllers/ItemController.cs
```

was replaced with:

```text
Controllers/GradesController.cs
```

## Refactored code

```csharp
using Microsoft.AspNetCore.Mvc;
using Siemens.Internship2026.GradeBook.Interfaces;

namespace Siemens.Internship2026.GradeBook.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GradesController : ControllerBase
{
    private readonly IGradeRepository _gradeRepository;
    private readonly IGradeStatisticsService _gradeStatisticsService;
    private readonly ILogger<GradesController> _logger;

    public GradesController(
        IGradeRepository gradeRepository,
        IGradeStatisticsService gradeStatisticsService,
        ILogger<GradesController> logger)
    {
        _gradeRepository = gradeRepository;
        _gradeStatisticsService = gradeStatisticsService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("GET api/grades called at {Time}", DateTime.UtcNow);

        var grades = await _gradeRepository.GetAllAsync();
        var response = _gradeStatisticsService.BuildResponse(grades);

        _logger.LogInformation(
            "Returning {TotalCount} grades with average value {AverageValue}",
            response.Statistics.TotalCount,
            response.Statistics.AverageValue);

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        _logger.LogInformation("GET api/grades/{Id} called at {Time}", id, DateTime.UtcNow);

        if (id <= 0)
        {
            _logger.LogWarning("Invalid grade id: {Id}", id);

            return BadRequest("Id must be a positive integer.");
        }

        var grade = await _gradeRepository.GetByIdAsync(id);

        if (grade == null)
        {
            _logger.LogWarning("Grade with id {Id} was not found.", id);

            return NotFound($"Grade with Id {id} was not found.");
        }

        return Ok(grade);
    }
}
```

## SOLID principles improved

### Single Responsibility Principle

The controller now only coordinates the HTTP request and response.

It no longer calculates statistics directly.

### Dependency Inversion Principle

The controller depends on abstractions:

```csharp
IGradeRepository
IGradeStatisticsService
ILogger<GradesController>
```

It does not create concrete classes manually.

## Why this is better

The controller is now easier to understand:

```text
Request comes in.
Controller asks repository for data.
Controller asks service to build statistics.
Controller returns response.
```

---


# 7. Replacing `Console.WriteLine` with `ILogger`

## Original problem

The original controller used:

```csharp
Console.WriteLine($"[LOG] {DateTime.UtcNow}: GET api/item called");
```

This is not recommended in ASP.NET Core applications.

ASP.NET Core provides a built-in logging abstraction called `ILogger<T>`.

## SOLID principle involved

This improves separation of concerns and supports the **Single Responsibility Principle**.

The controller should not be tied to console output. It should use the logging abstraction provided by the framework.

## Applied change

The controller now receives a logger through dependency injection:

```csharp
private readonly ILogger<GradesController> _logger;
```

and uses it like this:

```csharp
_logger.LogInformation("GET api/grades called at {Time}", DateTime.UtcNow);
```

## Why this is better

`ILogger` is configurable and works better in real web applications.

Logs can later be redirected to:

- console;
- files;
- cloud logging;
- monitoring tools.

---

# 8. Fixing Dependency Injection in `Program.cs`

## Original problem

The original `Program.cs` registered only controllers:

```csharp
builder.Services.AddControllers();
```

However, the controller required dependencies through its constructor.

If ASP.NET Core does not know how to create those dependencies, the application can fail at runtime.

## SOLID principle involved

This is connected to the **Dependency Inversion Principle**.

Depending on interfaces is good, but the concrete implementations must be registered in the dependency injection container.

## Applied change

`Program.cs` was updated to register the required services.

## Refactored code

```csharp
using Siemens.Internship2026.GradeBook.Interfaces;
using Siemens.Internship2026.GradeBook.Repositories;
using Siemens.Internship2026.GradeBook.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSingleton<IGradeRepository, InMemoryGradeRepository>();
builder.Services.AddScoped<IGradeStatisticsService, GradeStatisticsService>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
```

## Why this is better

The dependency injection container now knows:

```text
IGradeRepository -> InMemoryGradeRepository
IGradeStatisticsService -> GradeStatisticsService
```

So when ASP.NET Core creates `GradesController`, it can inject the correct dependencies.

---

# 9. Updating the API Route

## Original route

The original route was:

```text
api/item
```

## New route

After renaming `ItemController` to `GradesController`, the route became:

```text
api/grades
```

This is because of the route attribute:

```csharp
[Route("api/[controller]")]
```

In ASP.NET Core, `[controller]` uses the controller name without the `Controller` suffix.

So:

```text
GradesController -> grades
```

## Applied change

If `Properties/launchSettings.json` contained:

```json
"launchUrl": "api/item"
```

it was updated to:

```json
"launchUrl": "api/grades"
```

## Why this is better

The route now matches the domain of the project.

Before:

```text
/api/item
```

After:

```text
/api/grades
```

---

# 10. SOLID Principles After Refactoring

## Single Responsibility Principle

Each class now has a clearer responsibility.

```text
GradesController
```

Handles HTTP requests and responses.

```text
InMemoryGradeRepository
```

Handles data access.

```text
GradeStatisticsService
```

Calculates statistics and builds the response.

```text
Grade
GradeStatistics
GradeBookResponse
```

Represent data models.

This is better than having the controller do everything.

---

## Open/Closed Principle

The code is easier to extend without modifying the controller.

For example, if new statistics are needed, such as:

- minimum grade;
- maximum grade;
- number of grades above 9;

these changes can be made mainly in `GradeStatisticsService`.

The controller does not need to know the details of how statistics are calculated.

---

## Liskov Substitution Principle

The controller depends on the interface:

```csharp
IGradeRepository
```

This means another implementation could replace `InMemoryGradeRepository`.

For example:

```csharp
public class DatabaseGradeRepository : IGradeRepository
{
    public Task<Grade?> GetByIdAsync(int id)
    {
        // read from database
    }

    public Task<IEnumerable<Grade>> GetAllAsync()
    {
        // read from database
    }
}
```

As long as the new class respects the interface contract, the controller can use it without changes.

---

## Interface Segregation Principle

The interfaces are small and focused.

```csharp
public interface IGradeRepository
{
    Task<Grade?> GetByIdAsync(int id);

    Task<IEnumerable<Grade>> GetAllAsync();
}
```

```csharp
public interface IGradeStatisticsService
{
    GradeBookResponse BuildResponse(IEnumerable<Grade> grades);
}
```

No class is forced to implement unnecessary methods.

---

## Dependency Inversion Principle

The controller depends on abstractions, not concrete implementations.

It depends on:

```csharp
IGradeRepository
IGradeStatisticsService
ILogger<GradesController>
```

The concrete classes are configured in `Program.cs`.

This makes the code easier to test and easier to change later.

---
---

## Step 11: Upgrade the Project from .NET 8 to .NET 10

The assignment required upgrading the project from .NET 8 to .NET 10.

### Original problem

The project originally targeted .NET 8:

```xml
<TargetFramework>net8.0</TargetFramework>
```

### Applied change

The target framework was updated in the `.csproj` file:

```xml
<TargetFramework>net10.0</TargetFramework>
```

### Why this change was made

The application now targets .NET 10, as required by the assignment.

After the change, the project can be built and run using the .NET 10 SDK.

### Related commit

```text
Upgrade project to .NET 10
```

---

## Step 12: Replace the In-Memory Repository with an External HTTP Repository

The assignment required replacing the current in-memory data source with data fetched from an external endpoint.

### Original problem

The previous repository stored grades directly in memory:

```csharp
private readonly List<Grade> _grades = new()
{
    new Grade { Id = 1, Value = 9.50m },
    new Grade { Id = 2, Value = 8.75m }
};
```

This meant that the data was hardcoded inside the application.

### Applied change

The in-memory repository was replaced with `HttpGradeRepository`.

The new repository fetches grades from the external endpoint configured in `appsettings.json`.

```csharp
var grades = await _httpClient.GetFromJsonAsync<List<Grade>>(_options.Url);
```

The endpoint URL was moved to configuration:

```json
{
  "ExternalGradeSource": {
    "Url": "EXTERNAL_ENDPOINT_URL"
  }
}
```

### Why this change was made

The repository layer is responsible for data access. Since the data now comes from an external source, the repository was refactored to retrieve the data through HTTP instead of using a hardcoded list.

### SOLID principle involved

This supports the Single Responsibility Principle because the repository has one clear responsibility: fetching grade data.

It also supports the Dependency Inversion Principle because the rest of the application still depends on `IGradeRepository`, not directly on `HttpGradeRepository`.

### Related commit

```text
Replace in-memory repository with external HTTP repository
```

---

## Step 13: Add a Service Layer for Business Logic

The assignment required introducing a service layer that encapsulates the business logic.

### Original problem

The controller was previously closer to the data access layer. It either called the repository directly or contained business-related logic.

Business rules should not be placed inside the controller.

### Applied change

A new service interface and implementation were added:

```text
Interfaces/IGradeService.cs
Services/GradeService.cs
```

The service contains the business rule required by the assignment:

```text
Retrieve the first N grades that are active and passing.
```

A passing grade is a grade with a value greater than or equal to 5.

```csharp
return grades
    .Where(grade => grade.IsActive)
    .Where(grade => grade.Value >= 5m)
    .Take(count)
    .ToList();
```

### Why this change was made

The service layer separates business logic from the controller and repository.

The controller handles HTTP requests.

The repository fetches data.

The service applies business rules.

### SOLID principle involved

This improves the Single Responsibility Principle.

Each layer now has a clearer purpose:

```text
Controller -> HTTP request/response
Service -> business logic
Repository -> data access
Model -> data structure
```

### Related commit

```text
Add grade service with passing active grades filter
```

---

## Step 14: Add Endpoint for the First N Passing Active Grades

The assignment required that `N` is provided by the user.

### Applied change

A new endpoint was added to `GradesController`:

```csharp
[HttpGet("passing-active")]
public async Task<IActionResult> GetFirstPassingActiveGrades([FromQuery] int count)
```

The endpoint can be called like this:

```text
GET /api/grades/passing-active?count=3
```

This returns the first 3 grades that are:

```text
active
passing, meaning value >= 5
```

### Error handling

If the user provides an invalid count, for example:

```text
GET /api/grades/passing-active?count=0
```

the application returns a bad request response.

### Why this change was made

The endpoint exposes the required filtering functionality through the Web API.

The controller receives the value of `N` from the query parameter and delegates the filtering logic to the service layer.

### Related commit

```text
Use grade service from controller
```

---

## Step 15: Register the New Repository and Service Layer

After adding the new repository and service classes, they had to be registered in `Program.cs`.

### Applied change

The following registrations were added:

```csharp
builder.Services.Configure<ExternalGradeSourceOptions>(
    builder.Configuration.GetSection("ExternalGradeSource"));

builder.Services.AddHttpClient<IGradeRepository, HttpGradeRepository>();

builder.Services.AddScoped<IGradeService, GradeService>();
builder.Services.AddScoped<IGradeStatisticsService, GradeStatisticsService>();
```

### Why this change was made

ASP.NET Core uses dependency injection. This means that when the controller asks for `IGradeService`, the application must know which concrete class should be created.

These registrations connect the interfaces to their implementations.

### SOLID principle involved

This supports the Dependency Inversion Principle.

The controller depends on abstractions, and the concrete implementations are configured in one central place.

### Related commit

```text
Register external repository and service layer
```