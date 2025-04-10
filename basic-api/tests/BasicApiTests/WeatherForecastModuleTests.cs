using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BasicApi.Modules;
using Craft.CraftModule;
using Craft.CraftModule.Attributes;
using Craft.CraftModule.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BasicApiTests;

[DependsOn(typeof(WeatherForecastModule))]
public class TestModule : CraftModule
{
    public override IEndpointRouteBuilder AddRoutes(IEndpointRouteBuilder builder)
    {
        return builder;
    }
}

public class WeatherForecastModuleTests : IAsyncLifetime
{
    
    private WebApplicationBuilder _webApplicationBuilder;
    private WebApplication _webApplication;
    private IServiceScope _scope;
    private HttpClient _httpClient;
    
    [Fact(DisplayName = "Get a random weather forecast's summary")]
    public async Task Test1()
    {
        var response = await _httpClient.GetFromJsonAsync<WeatherForecast[]>("/api/forecast");
        Assert.NotNull(response);
        Assert.Equal(3, response.Length);
        Assert.All(response, forecast =>
        {
            Assert.NotNull(forecast);
            Assert.InRange(forecast.TemperatureC, -20, 55);
            Assert.NotNull(forecast.Summary);
        });
    }

    public async Task InitializeAsync()
    {
        _webApplicationBuilder = WebApplication.CreateBuilder();

        var services = _webApplicationBuilder.Services;
        services.AddHttpClient(
            "client",
            client =>
            {
                client.BaseAddress = new Uri("http://localhost:5000");
            }
        );
        services.AddLogging();
     

        services.AddCraftModules([typeof(TestModule)]);

        _webApplication = _webApplicationBuilder.Build();
        _webApplication.UseCraftGeneralException();

        _webApplication.MapCraftModulesEndpoint();

        await _webApplication.StartAsync();
        _scope = _webApplication.Services.CreateScope();
   
        _httpClient = _scope
            .ServiceProvider.GetRequiredService<IHttpClientFactory>()
            .CreateClient("client");
    }

    public async Task DisposeAsync()
    {
        _webApplication?.DisposeAsync();
    }
}