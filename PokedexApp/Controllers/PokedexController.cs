using Microsoft.AspNetCore.Mvc;
using Pokedex.Logging.Interfaces;

namespace PokedexApp.Controllers
{
    public class PokedexController : Controller
    {
        private ILoggerAdapter<PokedexController> _logger;

        public PokedexController(ILoggerAdapter<PokedexController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}