var builder = WebApplication.CreateBuilder(args);

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

app.MapGet("/secret-demo", (IConfiguration configuration) =>
{
    var configuredSecret = configuration["DemoSettings:Secret"] ?? "not-configured";
    var source = string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("DemoSettings__Secret"))
        ? "appsettings"
        : "environment";

    return Results.Ok(new
    {
        source,
        secretLength = configuredSecret.Length,
        preview = configuredSecret.Length <= 4
            ? "****"
            : $"{configuredSecret[..2]}***{configuredSecret[^2..]}"
    });
})
.WithName("GetSecretDemo");

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

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
