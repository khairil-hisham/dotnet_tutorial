var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello from .NET backend!");
app.MapGet("/user/{id}", (int id) => new { Id = id, Name = $"User{id}" });

app.Run();
