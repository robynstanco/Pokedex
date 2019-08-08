using Microsoft.AspNetCore.Mvc;

namespace Pokedex.Controllers
{
    public class MyPokedexController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}