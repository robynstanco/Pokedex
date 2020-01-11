using Microsoft.AspNetCore.Mvc;
using Pokedex.Common;
using Pokedex.Logging.Interfaces;
using PokedexApp.Interfaces;
using PokedexApp.Models;
using System;

namespace PokedexApp.Controllers
{
    public class PokedexController : Controller
    {
        private IPokedexAppLogic _pokedexAppLogic;
        private ILoggerAdapter<PokedexController> _logger;
        public PokedexController(IPokedexAppLogic pokedexAppLogic, ILoggerAdapter<PokedexController> logger)
        {
            _pokedexAppLogic = pokedexAppLogic;
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                return View(_pokedexAppLogic.GetMyPokedex());
            }
            catch(Exception ex)
            {
                return Error(ex);
            }
        }

        public IActionResult Detail(Guid id)
        {
            try
            {
                return View(_pokedexAppLogic.GetMyPokemonById(id));
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            return View(Constants.Error, new ErrorViewModel() { Message = ex.Message });
        }
    }
}