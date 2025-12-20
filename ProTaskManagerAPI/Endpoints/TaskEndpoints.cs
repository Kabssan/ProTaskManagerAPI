using Microsoft.EntityFrameworkCore;
using ProTaskManagerAPI.Data;
using ProTaskManagerAPI.Models;

namespace ProTaskManagerAPI.Endpoints;

public static class TaskEndpoints
{
    public static void MapTaskEndpoints(this IEndpointRouteBuilder app)
    {
        // Wir erstellen eine Gruppe, damit wir nicht überall "/api/tasks" schreiben müssen
        var group = app.MapGroup("/api/tasks");

        // GET: Alle Aufgaben abrufen (mit optionaler Suche)
        group.MapGet("/", async (string? search, AppDbContext db) =>
        {
            var query = db.Tasks.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(t => t.Title.ToLower().Contains(search.ToLower()));
            }

            return Results.Ok(await query.ToListAsync());
        });

        // POST: Neue Aufgabe erstellen (mit Validierung)
        group.MapPost("/", async (TodoTask task, AppDbContext db) =>
        {
            if (string.IsNullOrWhiteSpace(task.Title))
            {
                return Results.BadRequest("Der Titel darf nicht leer sein.");
            }

            db.Tasks.Add(task);
            await db.SaveChangesAsync();
            return Results.Created($"/api/tasks/{task.Id}", task);
        });

        // PUT: Aufgabe aktualisieren
        group.MapPut("/{id}", async (int id, TodoTask updatedTask, AppDbContext db) =>
        {
            var task = await db.Tasks.FindAsync(id);
            if (task is null) return Results.NotFound();

            task.Title = updatedTask.Title;
            task.Description = updatedTask.Description;
            task.IsCompleted = updatedTask.IsCompleted;

            task.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        // DELETE: Aufgabe löschen
        group.MapDelete("/{id}", async (int id, AppDbContext db) =>
        {
            var task = await db.Tasks.FindAsync(id);
            if (task is null) return Results.NotFound();

            db.Tasks.Remove(task);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });
    }
}