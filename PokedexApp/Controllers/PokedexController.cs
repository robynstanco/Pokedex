using Microsoft.AspNetCore.Mvc;
using Pokedex.Common;
using Pokedex.Logging.Interfaces;
using PokedexApp.Interfaces;
using PokedexApp.Models;
using System;
using System.Collections.Generic;

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

        public IActionResult Index()
        {
            try
            {
                List<PokemonListingViewModel> myPokemon = _pokedexAppLogic.GetMyPokedex();
                return View(myPokemon);
            }
            catch(Exception ex)
            {
                return Error(ex);
            }
        }

        public IActionResult Detail(Guid id)
        {
            try
            {
                PokemonDetailViewModel myPokemon = _pokedexAppLogic.GetMyPokemonById(id);

                return View(myPokemon);
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }

        public IActionResult Edit(Guid id)
        {
            try
            {
                PokemonDetailViewModel pokemonDetailViewModel = _pokedexAppLogic.GetMyPokemonById(id);
                pokemonDetailViewModel.IsEditMode = true;

                return View("Detail", pokemonDetailViewModel);
            }
            catch(Exception ex)
            {
                return Error(ex);
            }
        }

        [HttpPost]
        public IActionResult Edit(PokemonDetailViewModel pokemonDetailViewModel)
        {
            try
            {
                _pokedexAppLogic.EditPokemon(pokemonDetailViewModel);

                return View(Constants.Success, new SuccessViewModel() { ActionName = "edit" });
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }

        public IActionResult Delete(Guid id)
        {
            try
            {
                _pokedexAppLogic.DeletePokemonById(id);

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