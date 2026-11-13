using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using School.Infrastructure;
using School.Infrastructure.Context;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.DefaultIgnoreCondition =
            JsonIgnoreCondition.WhenWritingNull);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure(
    builder.Configuration.GetConnectionString("SchoolDatabase")
    ?? "Data Source=school.db");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

await using (var scope = app.Services.CreateAsyncScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SchoolContext>();
    await context.Database.EnsureCreatedAsync();
}

app.Run();

public partial class Program
{
}
