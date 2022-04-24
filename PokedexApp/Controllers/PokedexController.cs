using cloudscribe.Pagination.Models;
using Microsoft.AspNetCore.Mvc;
using Pokedex.Common;
using Pokedex.Common.Interfaces;
using Pokedex.Logging.Interfaces;
using PokedexApp.Interfaces;
using PokedexApp.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PokedexApp.Controllers
{
    /// <summary>
    /// The Pokédex Controller.
    /// </summary>
    public class PokedexController : Controller
    {
        private ILoggerAdapter<PokedexController> _logger;
        private IPaginationHelper _paginationHelper;
        private IPokedexAppLogic _pokedexAppLogic;
        public PokedexController(ILoggerAdapter<PokedexController> logger, IPaginationHelper paginationHelper, IPokedexAppLogic pokedexAppLogic)
        {
            _logger = logger;
            _paginationHelper = paginationHelper;
            _pokedexAppLogic = pokedexAppLogic;
        }

        /// <summary>
        /// The main Pokédex page with paginated results.
        /// </summary>
        /// <param name="pageNumber">The page number for pagination.</param>
        /// <param name="pageSize">The page size for pagination.</param>
        /// <returns>The paginated results.</returns>
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = Constants.PageSize)
        {
            try
            {
                IEnumerable<PokemonListingViewModel> myPokemon = await _pokedexAppLogic.GetMyPokedex();

                PagedResult<PokemonListingViewModel> pagedNationalDex = _paginationHelper.GetPagedResults(myPokemon, pageNumber, pageSize);

                return View(pagedNationalDex);
            }
            catch(Exception ex)
            {
                return Error(ex);
            }
        }

        /// <summary>
        /// The detail page for a given guid.
        /// </summary>
        /// <param name="id">The guid.</param>
        /// <returns>The detail view.</returns>
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

        /// <summary>
        /// The edit action for given Guid.
        /// </summary>
        /// <param name="id">The guid to edit.</param>
        /// <returns>The edit view.</returns>
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

        /// <summary>
        /// The edit action for a given Pokémon Detail.
        /// </summary>
        /// <param name="pokemonDetailViewModel">The detail to edit.</param>
        /// <returns>The edited detail.</returns>
        [HttpPost]
        public async Task<IActionResult> Edit(PokemonDetailViewModel pokemonDetailViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _pokedexAppLogic.EditPokemon(pokemonDetailViewModel);

                    return View(Constants.Success, new SuccessViewModel() { ActionName = "edit" });
                }
                else
                {
                    _logger.LogInformation(Constants.InvalidRequest);

                    return await Edit(pokemonDetailViewModel.MyPokemonId.Value);
                }
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }

        /// <summary>
        /// The delete action for a given Guid.
        /// </summary>
        /// <param name="id">The guid to delete.</param>
        /// <returns>The success result.</returns>
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