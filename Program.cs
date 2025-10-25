using System.Linq;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();

var app = builder.Build();

app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod());

app.MapGet("/", () => "Hello from .NET!");

app.MapGet("/sum", (int number) =>
{
    int sum = number
        .ToString()
        .Where(char.IsDigit)
        .Sum(c => c - '0');

    return Results.Json(new { number, sum });
});

app.Run();
