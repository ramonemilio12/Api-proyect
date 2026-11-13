using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using BiciTaller.Application;
using BiciTaller.Infrastructure;
using BiciTaller.Infrastructure.Context;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.DefaultIgnoreCondition =
            JsonIgnoreCondition.WhenWritingNull);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure(
    builder.Configuration.GetConnectionString("BiciTallerDatabase")
    ?? "Data Source=bicitaller.db");
builder.Services.AddApplication();

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
    var context = scope.ServiceProvider.GetRequiredService<BiciTallerContext>();
    await context.Database.EnsureCreatedAsync();
}

app.Run();

public partial class Program
{
}
