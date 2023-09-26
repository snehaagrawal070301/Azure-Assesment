using Azure.Data.AppConfiguration;
using Azure.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WeaatherForecastApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<string> Get()
        {
            try
            {
                List<string> strings = new List<string>();
                string endpoint = "https://rg-app-config-test.azconfig.io"; // Replace with your App Configuration endpoint
                var client = new ConfigurationClient(new Uri(endpoint), new ManagedIdentityCredential());
                foreach (ConfigurationSetting setting in client.GetConfigurationSettings(new SettingSelector()))
                {
                    strings.Add($"Key: {setting.Key}, Value: {setting.Value}");
                }
                return strings;
                // Code to interact with Azure App Configuration
            }
            catch (Exception ex)
            {
                throw ex.InnerException;

            }
           
        }
    }
}