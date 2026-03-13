using Application.Dtos.RequestDto;
using Asp.Versioning;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.Extensions;
using Infrastructure.Persistence.Seeders;
using WebAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container

// Controllers with FluentValidation
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<StudentRequestValidator>();

// API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("X-Api-Version"),
        new QueryStringApiVersionReader("api-version"));
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Student Registration API",
        Version = "v1",
        Description = "A RESTful API for student registration management demonstrating ASP.NET Core best practices."
    });
});

// Infrastructure services (Database, Repositories, Services, AutoMapper)
builder.Services.AddInfrastructure(configuration);

var app = builder.Build();

// Configure the HTTP request pipeline

// Global exception handling (before other middleware)
app.UseExceptionHandling();

// Swagger UI (Development only)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Student Registration API v1");
        options.RoutePrefix = "swagger";
    });

    // Seed database in development
    var seedDatabase = configuration.GetValue<bool>("SeedDatabase");
    if (seedDatabase)
    {
        await StudentSeeder.SeedAsync(app.Services);
    }
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Log startup information
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Student Registration API started. Environment: {Environment}", app.Environment.EnvironmentName);

app.Run();
