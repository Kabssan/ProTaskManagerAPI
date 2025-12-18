using Microsoft.EntityFrameworkCore;
using ProTaskManagerAPI.Data;
using ProTaskManagerAPI.Models;

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

app.MapGet("/api/tasks", async (string? search, AppDbContext db) =>
{
    // Starten mit der Basis-Abfrage (alle Aufgaben)
    var query = db.Tasks.AsQueryable();

    // Wenn der User einen Suchbegriff eingegeben hat:
    if (!string.IsNullOrWhiteSpace(search))
    {
        // Filtern: Der Titel muss den Suchbegriff enthalten
        query = query.Where(t => t.Title.ToLower().Contains(search.ToLower()));
    }

    // Am Ende wird die gefilterte (oder ungefilterte) Abfrage ausgeführt
    return Results.Ok(await query.ToListAsync());
});

// Diese Funktion erlaubt es uns, neue Aufgaben in die Datenbank zu schreiben
app.MapPost("/api/tasks", async (ProTaskManagerAPI.Models.TodoTask task, ProTaskManagerAPI.Data.AppDbContext db) =>
{
    // Check: Ist der Titel leer oder nur Leerzeichen?
    if (string.IsNullOrWhiteSpace(task.Title))
    {
        return Results.BadRequest("Der Titel darf nicht leer sein!");
    }

    db.Tasks.Add(task);
    await db.SaveChangesAsync();
    return Results.Created($"/api/tasks/{task.Id}", task);
});

// UPDATE: Eine bestehende Aufgabe ändern
app.MapPut("/api/tasks/{id}", async (int id, TodoTask updatedTask, AppDbContext db) =>
{
    var task = await db.Tasks.FindAsync(id);

    if (task is null) return Results.NotFound();

    // Felder aktualisieren
    task.Title = updatedTask.Title;
    task.Description = updatedTask.Description;
    task.IsCompleted = updatedTask.IsCompleted;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

// DELETE: Eine Aufgabe anhand ihrer ID löschen
app.MapDelete("/api/tasks/{id}", async (int id, AppDbContext db) =>
{
    var task = await db.Tasks.FindAsync(id);
    
    if (task is null) return Results.NotFound();

    db.Tasks.Remove(task);
    await db.SaveChangesAsync();
    
    return Results.NoContent(); // 204 No Content ist die Standard-Antwort für erfolgreiches Löschen
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
