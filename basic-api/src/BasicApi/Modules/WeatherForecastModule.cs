using Craft.CraftModule;
using Microsoft.AspNetCore.Mvc;

namespace BasicApi.Modules;

public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

public sealed class WeatherForecastModule : CraftModule
{
    private readonly string[] _summaries =
    [
        "Freezing",
        "Bracing",
        "Chilly",
        "Cool",
        "Mild",
        "Warm",
        "Balmy",
        "Hot",
        "Sweltering",
        "Scorching",
    ];

    public override IEndpointRouteBuilder AddRoutes(IEndpointRouteBuilder builder)
    {
        var endpoint = builder
            .MapGet(
                "/api/forecast",
                (int? count) =>
                {
                    var forecast = Enumerable
                        .Range(1, count ?? 3)
                        .Select(index =>
                        {
                            var date = DateOnly.FromDateTime(DateTime.Now.AddDays(index));
                            var temperatureC = Random.Shared.Next(-20, 55);
                            var summary = _summaries[Random.Shared.Next(_summaries.Length)];
                            return new WeatherForecast(date, temperatureC, summary);
                        })
                        .ToArray();

                    return Results.Ok(forecast);
                }
            )
            .WithDisplayName("Weather Forecast")
            .WithDescription("Get a random weather forecast's summary")
            .WithTags("Weather")
            .Produces<WeatherForecast[]>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        return builder;
    }
}
