using Microsoft.AspNetCore.Mvc;
namespace TestProj.Controllers
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
            
            _logger.LogWarning("Ak An example of a Warning trace..");
            _logger.LogError("Ak An example of an Error level message");

            // Log "Hello World" using ILogger
            _logger.LogDebug("Ak Debug Hello World");
            _logger.LogInformation("Ak Info Hello World");
            return View();
        }
    }
}