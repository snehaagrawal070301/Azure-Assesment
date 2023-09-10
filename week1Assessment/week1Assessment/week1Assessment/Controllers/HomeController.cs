using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using week1Assessment.Models;

namespace week1Assessment.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var builder = new ConfigurationBuilder();
            builder.AddAzureAppConfiguration("Endpoint=https://snehaappconfig.azconfig.io;Id=/xqc;Secret=VAQMoWptnATSLUW+0fce14sXA5b/e2T/XIpyTHp1bQk=");
            var config = builder.Build();
            var data = new Dictionary<string, object>();
            foreach(var kvp in config.AsEnumerable())
            {
                data[kvp.Key] = kvp.Value; 
            }
            return View(data);
        }

        public IActionResult Privacy()
        {
            _logger.LogWarning("Warning example");
            _logger.LogError("An error occured");
            _logger.LogDebug("debug app");
            _logger.LogInformation("information of app");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}