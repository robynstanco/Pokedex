using Microsoft.AspNetCore.Mvc;
using Pokedex.Common;
using Pokedex.Common.Interfaces;
using Pokedex.Logging.Interfaces;
using PokedexApp.Interfaces;
using PokedexApp.Models;
using System;
using System.Threading.Tasks;

namespace PokedexApp.Controllers
{
    /// <summary>
    /// The National Dex Controller.
    /// </summary>
    public class NationalDexController : Controller
    {
        private readonly ILoggerAdapter<NationalDexController> _logger;
        private readonly IPaginationHelper _paginationHelper;
        private readonly IPokedexAppLogic _pokedexAppLogic;
        public NationalDexController(ILoggerAdapter<NationalDexController> logger, IPaginationHelper paginationHelper, IPokedexAppLogic pokedexAppLogic)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _paginationHelper = paginationHelper ?? throw new ArgumentNullException(nameof(paginationHelper));
            _pokedexAppLogic = pokedexAppLogic ?? throw new ArgumentNullException(nameof(pokedexAppLogic));
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
                var nationalDex = await _pokedexAppLogic.GetNationalDex();

                var pagedNationalDex = _paginationHelper.GetPagedResults(nationalDex, pageNumber, pageSize);

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
                var nationalDexPokemon = await _pokedexAppLogic.GetNationalDexPokemonById(id);

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

            return View(Constants.Error, new ErrorViewModel { Message = ex.Message });
        }
    }
}