using IsLabApp.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

var notes = new List<Note>();
var nextId = 1;

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapPost("/api/notes", (Note input) =>
{
    var note = new Note
    {
        Id = nextId++,
        Title = input.Title,
        Text = input.Text
    };
    notes.Add(note);
    return Results.Created($"/api/notes/{note.Id}", note);
});

app.MapGet("/api/notes", () => notes);

app.MapGet("/api/notes/{id}", (int id) =>
{
    var note = notes.FirstOrDefault(n => n.Id == id);
    return note is null ? Results.NotFound() : Results.Ok(note);
});

app.MapDelete("/api/notes/{id}", (int id) =>
{
    var note = notes.FirstOrDefault(n => n.Id == id);
    if (note is null) return Results.NotFound();
    notes.Remove(note);
    return Results.NoContent();
});

app.MapGet("/health", () => new
{
    status = "ok",
    time = DateTime.UtcNow
});

app.MapGet("/version", () => new
{
    appName = "IsLabApp",
    version = "0.1.0-lab4"
});

app.MapGet("/db/ping", () => new
{
    status = "error",
    message = "SQL Server not configured yet"
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}