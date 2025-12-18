using Microsoft.EntityFrameworkCore;
using ProTaskManagerAPI.Data;
using ProTaskManagerAPI.Endpoints;

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

//app.UseHttpsRedirection();

app.MapTaskEndpoints();

app.Run();

