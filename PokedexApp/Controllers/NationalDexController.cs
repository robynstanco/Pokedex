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
    public class NationalDexController : Controller
    {
        private ILoggerAdapter<NationalDexController> _logger;
        private IPaginationHelper _paginationHelper;
        private IPokedexAppLogic _pokedexAppLogic;
        public NationalDexController(ILoggerAdapter<NationalDexController> logger, IPaginationHelper paginationHelper, IPokedexAppLogic pokedexAppLogic)
        {
            _logger = logger;
            _paginationHelper = paginationHelper;
            _pokedexAppLogic = pokedexAppLogic;
        }

        /// <summary>
        /// The main NationalDex page with paged results.
        /// </summary>
        /// <param name="pageNumber">The page number for pagination.</param>
        /// <param name="pageSize">The page size for pagination.</param>
        /// <returns>The paginated NationalDex results.</returns>
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = Constants.PageSize)
        {
            try
            {
                IEnumerable<PokemonListingViewModel> nationalDex = await _pokedexAppLogic.GetNationalDex();

                PagedResult<PokemonListingViewModel> pagedNationalDex = _paginationHelper.GetPagedResults(nationalDex, pageNumber, pageSize);

                return View(pagedNationalDex);
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }

        /// <summary>
        /// The NationalDex detail page.
        /// </summary>
        /// <param name="id">The national dex id.</param>
        /// <returns>The NationalDex result.</returns>
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