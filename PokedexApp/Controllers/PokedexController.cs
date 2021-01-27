using cloudscribe.Pagination.Models;
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
    public class PokedexController : Controller
    {
        private IPokedexAppLogic _pokedexAppLogic;
        private ILoggerAdapter<PokedexController> _logger;
        public PokedexController(IPokedexAppLogic pokedexAppLogic, ILoggerAdapter<PokedexController> logger)
        {
            _pokedexAppLogic = pokedexAppLogic;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = Constants.PageSize)
        {
            try
            {
                IEnumerable<PokemonListingViewModel> myPokemon = await _pokedexAppLogic.GetMyPokedex();

                PagedResult<PokemonListingViewModel> pagedNationalDex =
                    _pokedexAppLogic.GetPagedResults(myPokemon, pageNumber, pageSize);

                return View(pagedNationalDex);
            }
            catch(Exception ex)
            {
                return Error(ex);
            }
        }

        public async Task<IActionResult> Detail(Guid id)
        {
            try
            {
                PokemonDetailViewModel myPokemon = await _pokedexAppLogic.GetMyPokemonById(id);

                return View(myPokemon);
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                PokemonDetailViewModel pokemonDetailViewModel = await _pokedexAppLogic.GetMyPokemonById(id);
                pokemonDetailViewModel.IsEditMode = true;

                return View(Constants.Detail, pokemonDetailViewModel);
            }
            catch(Exception ex)
            {
                return Error(ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PokemonDetailViewModel pokemonDetailViewModel)
        {
            try
            {
                await _pokedexAppLogic.EditPokemon(pokemonDetailViewModel);

                return View(Constants.Success, new SuccessViewModel() { ActionName = "edit" });
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _pokedexAppLogic.DeletePokemonById(id);

                return View(Constants.Success, new SuccessViewModel() { ActionName = "release" });
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