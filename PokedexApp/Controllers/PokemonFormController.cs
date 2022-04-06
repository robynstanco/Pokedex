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
        private ILoggerAdapter<PokemonFormController> _logger;
        private IPokedexAppLogic _pokedexAppLogic;
        public PokemonFormController(ILoggerAdapter<PokemonFormController> logger, IPokedexAppLogic pokedexAppLogic)
        {
            _logger = logger;
            _pokedexAppLogic = pokedexAppLogic;
        }

        /// <summary>
        /// The form for adding a Pokémon to personal Pokédex.
        /// </summary>
        /// <returns>The form.</returns>
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

        /// <summary>
        /// The add action for given form data.
        /// </summary>
        /// <param name="pokemonFormViewModel">The form view model.</param>
        /// <returns>The success result.</returns>
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

        /// <summary>
        /// The generic error page.
        /// </summary>
        /// <param name="ex">The exception.</param>
        /// <returns>The error result.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            return View(Constants.Error, new ErrorViewModel() { Message = ex.Message });
        }
    }
}