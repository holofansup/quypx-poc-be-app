using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using cirrus.Models;

namespace cirrusRapiapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherRapidapiController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        public WeatherRapidapiController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet]
        public async Task<IActionResult> GetWeatherFromRapidapi(string? city, string? lang)
        {
            if (city == string.Empty) { return StatusCode(500, "City Can Not Null"); }
            if (lang == string.Empty) { lang = "EN"; }

            var API_KEY = Environment.GetEnvironmentVariable("RAPIDAPI_KEY");
            var API_URI = $"https://open-weather13.p.rapidapi.com/city/{city}/{lang}";
            var HOST_NAME = "open-weather13.p.rapidapi.com";

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(API_URI),
                Headers =
                {
                    { "x-rapidapi-key", API_KEY },
                    { "x-rapidapi-host", HOST_NAME },
                },
            };

            try
            {
                using (var response = await _httpClient.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    var body = await response.Content.ReadAsStringAsync();
                    WeatherData weatherData = JsonConvert.DeserializeObject<WeatherData>(body);
                    return Ok(new { statusCode = 200, message = "Success", data = weatherData } );
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching weather data: {ex.Message}");
            }
        }
    }
}