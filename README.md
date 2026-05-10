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