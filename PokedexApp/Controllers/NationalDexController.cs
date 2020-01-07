using Microsoft.AspNetCore.Mvc;
using Pokedex.Common;
using Pokedex.Logging.Interfaces;
using PokedexApp.Interfaces;
using PokedexApp.Models;
using System;

namespace PokedexApp.Controllers
{
    public class NationalDexController : Controller
    {
        private IPokedexAppLogic _pokedexAppLogic;
        private ILoggerAdapter<NationalDexController> _logger;
        public NationalDexController(IPokedexAppLogic PokedexAppLogic, ILoggerAdapter<NationalDexController> logger)
        {
            _pokedexAppLogic = PokedexAppLogic;
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                return View(_pokedexAppLogic.GetNationalDex());
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }

        public IActionResult Detail(int id)
        {
            try
            {
                return View(_pokedexAppLogic.GetNationalDexPokemonById(id));
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