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