using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TaskManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Miled", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private static readonly List<WeatherForecast> _forecasts = new List<WeatherForecast>
        {
            new WeatherForecast{Id = 1, Date = DateOnly.FromDateTime(DateTime.Now), TemperatureC = 25, Summary = "Warm"}
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger) 
        {
            _logger = logger;
        }

        [HttpGet("weatherForcast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            }).ToArray();
        }

        // Endpoint to get all weather forecast
        [HttpGet("")]
        public IEnumerable<WeatherForecast> GetAll()
        {
            return _forecasts;
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            WeatherForecast forecast = _forecasts.FirstOrDefault(x => x.Id == id);
            if (forecast == null)
            {
                return NotFound();
            }

            _forecasts.Remove(forecast);
            return NoContent();
        }
    }
}
