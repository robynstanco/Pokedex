using Microsoft.AspNetCore.Mvc;
using Pokedex.Common;
using Pokedex.Logging.Interfaces;
using PokedexApp.Interfaces;
using PokedexApp.Models;
using System;
using System.Threading.Tasks;

namespace PokedexApp.Controllers
{
    /// <summary>
    /// The Search Controller.
    /// </summary>
    public class SearchController : Controller
    {
        private readonly ILoggerAdapter<SearchController> _logger;
        private readonly IPokedexAppLogic _pokedexAppLogic;
        public SearchController(ILoggerAdapter<SearchController> logger, IPokedexAppLogic pokedexAppLogic)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _pokedexAppLogic = pokedexAppLogic ?? throw new ArgumentNullException(nameof(pokedexAppLogic));
        }

        /// <summary>
        /// The search form page.
        /// </summary>
        /// <returns>The empty search form.</returns>
        public async Task<IActionResult> Index()
        {
            try
            {
                var searchViewModel = await _pokedexAppLogic.GetSearchForm();

                return View(searchViewModel);
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }

        /// <summary>
        /// The search submission.
        /// </summary>
        /// <param name="searchViewModel">The search criteria.</param>
        /// <returns>The search results.</returns>
        [HttpPost]
        public async Task<IActionResult> Index(SearchViewModel searchViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var results = await _pokedexAppLogic.Search(searchViewModel);

                    return View(results);
                }

                _logger.LogInformation(Constants.InvalidRequest);

                return await Index();
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