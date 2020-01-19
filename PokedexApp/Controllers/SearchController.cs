using Microsoft.AspNetCore.Mvc;
using Pokedex.Common;
using Pokedex.Logging.Interfaces;
using PokedexApp.Interfaces;
using PokedexApp.Models;
using System;
using System.Collections.Generic;

namespace PokedexApp.Controllers
{
    public class SearchController : Controller
    {
        private IPokedexAppLogic _pokedexAppLogic;
        private ILoggerAdapter<SearchController> _logger;
        public SearchController(IPokedexAppLogic pokedexAppLogic, ILoggerAdapter<SearchController> logger)
        {
            _pokedexAppLogic = pokedexAppLogic;
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                return View(_pokedexAppLogic.GetSearchForm());
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }

        [HttpPost]
        public IActionResult Index(SearchViewModel searchViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return View(_pokedexAppLogic.Search(searchViewModel));
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