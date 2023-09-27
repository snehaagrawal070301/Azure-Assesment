using Azure.Data.AppConfiguration;
using Azure.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Demo_Az_AD.Controllers
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

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<string> Get()
        {
            List<string> strings = new List<string>();
            string endpoint = "https://rg-test-app-config.azconfig.io"; // Replace with your App Configuration endpoint
            var client = new ConfigurationClient(new Uri(endpoint), new ManagedIdentityCredential());
            foreach (ConfigurationSetting setting in client.GetConfigurationSettings(new SettingSelector()))
            {
                strings.Add($"Key: {setting.Key}, Value: {setting.Value}");
            }
            return strings;
        }
    }
}