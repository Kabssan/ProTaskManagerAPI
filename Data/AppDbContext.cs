using Microsoft.EntityFrameworkCore;
using ProTaskManagerAPI.Models;

namespace ProTaskManagerAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Das wird deine Tabelle in der Datenbank
    public DbSet<TodoTask> Tasks { get; set; } = null!;
}