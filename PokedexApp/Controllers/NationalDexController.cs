using Microsoft.AspNetCore.Mvc;
using Pokedex.Common;
using Pokedex.Logging.Interfaces;
using PokedexApp.Interfaces;
using PokedexApp.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PokedexApp.Controllers
{
    public class NationalDexController : Controller
    {
        private IPokedexAppLogic _pokedexAppLogic;
        private ILoggerAdapter<NationalDexController> _logger;
        public NationalDexController(IPokedexAppLogic pokedexAppLogic, ILoggerAdapter<NationalDexController> logger)
        {
            _pokedexAppLogic = pokedexAppLogic;
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                List<PokemonListingViewModel> nationalDex = _pokedexAppLogic.GetNationalDex();

                return View(nationalDex);
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }

        public async Task<IActionResult> Detail(int id)
        {
            try
            {
                PokemonDetailViewModel nationalDexPokemon = await _pokedexAppLogic.GetNationalDexPokemonById(id);

                return View(nationalDexPokemon);
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