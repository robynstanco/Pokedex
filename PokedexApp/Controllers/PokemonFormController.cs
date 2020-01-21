using Microsoft.AspNetCore.Mvc;
using Pokedex.Common;
using Pokedex.Logging.Interfaces;
using PokedexApp.Interfaces;
using PokedexApp.Models;
using System;

namespace PokedexApp.Controllers
{
    public class PokemonFormController : Controller
    {
        private IPokedexAppLogic _pokedexAppLogic;
        private ILoggerAdapter<PokemonFormController> _logger;
        public PokemonFormController(IPokedexAppLogic pokedexAppLogic, ILoggerAdapter<PokemonFormController> logger)
        {
            _pokedexAppLogic = pokedexAppLogic;
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                return View(_pokedexAppLogic.GetNewPokemonForm());
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }

        [HttpPost]
        public IActionResult Index(PokemonFormViewModel pokemonFormViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _pokedexAppLogic.AddPokemon(pokemonFormViewModel);

                    return View(Constants.Success, new SuccessViewModel() { ActionName = "addition" });
                }
                else
                {
                    _logger.LogInformation(Constants.InvalidRequest);

                    return Index();
                }
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