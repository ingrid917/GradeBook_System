using Siemens.Internship2026.GradeBook.Configuration;
using Siemens.Internship2026.GradeBook.Interfaces;
using Siemens.Internship2026.GradeBook.Repositories;
using Siemens.Internship2026.GradeBook.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.Configure<ExternalGradeSourceOptions>(
    builder.Configuration.GetSection("ExternalGradeSource"));

builder.Services.AddHttpClient<IGradeRepository, HttpGradeRepository>();

builder.Services.AddScoped<IGradeService, GradeService>();
builder.Services.AddScoped<IGradeStatisticsService, GradeStatisticsService>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();