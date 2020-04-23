using Microsoft.AspNetCore.Mvc;
using Pokedex.Common;
using Pokedex.Logging.Interfaces;
using PokedexApp.Interfaces;
using PokedexApp.Models;
using System;
using System.Threading.Tasks;

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

        public async Task<IActionResult> Index()
        {
            try
            {
                PokemonFormViewModel pokemonFormViewModel = await _pokedexAppLogic.GetNewPokemonForm();

                return View(pokemonFormViewModel);
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Index(PokemonFormViewModel pokemonFormViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _pokedexAppLogic.AddPokemon(pokemonFormViewModel);

                    return View(Constants.Success, new SuccessViewModel() { ActionName = "addition" });
                }
                else
                {
                    _logger.LogInformation(Constants.InvalidRequest);

                    return await Index();
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