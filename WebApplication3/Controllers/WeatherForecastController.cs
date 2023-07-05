using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebApplication3.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<double> Get(double lat, double lon)
        {
            var httpClient = _httpClientFactory.CreateClient();

            // Set up the URL with the provided lat and lon parameters
            var apiUrl = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid=95a74227ce18a56d4569f80efadfc031";

            // Send the API request and retrieve the JSON response
            var response = await httpClient.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            // Read the content as a string
            var content = await response.Content.ReadAsStringAsync();

            // Extract the temperature from the JSON response
            var temperatureC = ExtractTemperatureFromJson(content);

            return Math.Round(temperatureC, 2);
        }

        private static double ExtractTemperatureFromJson(string json)
        {
            // Parse the JSON string
            var jsonObject = System.Text.Json.JsonDocument.Parse(json).RootElement;

            // Extract the temperature value
            var temperatureK = jsonObject.GetProperty("main").GetProperty("temp").GetDouble();

            // Convert the temperature from Kelvin to Celsius
            var temperatureC = ConvertKelvinToCelsius(temperatureK);

            return temperatureC;
        }

        private static double ConvertKelvinToCelsius(double temperatureK)
        {
            return temperatureK - 273.15;
        }
    }
}