using Microsoft.EntityFrameworkCore;
using ProTaskManagerAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// ...

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=tasks.db"));

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();



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

app.MapGet("/api/tasks", () =>
{
    var testTasks = new List<ProTaskManagerAPI.Models.TodoTask>
    {
        new ProTaskManagerAPI.Models.TodoTask { Id = 1, Title = "Minimal API lernen", IsCompleted = true },
        new ProTaskManagerAPI.Models.TodoTask { Id = 2, Title = "Projekt erfolgreich starten", IsCompleted = false }
    };

    return Results.Ok(testTasks);
});

// Diese Funktion erlaubt es uns, neue Aufgaben in die Datenbank zu schreiben
app.MapPost("/api/tasks", async (ProTaskManagerAPI.Models.TodoTask task, ProTaskManagerAPI.Data.AppDbContext db) =>
{
    db.Tasks.Add(task);
    await db.SaveChangesAsync();
    return Results.Created($"/api/tasks/{task.Id}", task);
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
