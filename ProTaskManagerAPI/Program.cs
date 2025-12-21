using Microsoft.EntityFrameworkCore;
using ProTaskManagerAPI.Data;
using ProTaskManagerAPI.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", policy => {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();



// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty; // Swagger erscheint direkt beim Aufruf der URL
});

//app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.MapTaskEndpoints();



app.MapControllers();

app.Run();

