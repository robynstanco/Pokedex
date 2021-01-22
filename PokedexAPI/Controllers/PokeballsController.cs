﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pokedex.Common;
using Pokedex.Logging.Interfaces;
using PokedexAPI.Interfaces;
using PokedexAPI.Models.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PokedexAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PokeballsController : ControllerBase
    {
        private IPokedexAPILogic _pokedexAPILogic;
        private ILoggerAdapter<PokeballsController> _logger;
        public PokeballsController(IPokedexAPILogic pokedexAPILogic, ILoggerAdapter<PokeballsController> logger)
        {
            _pokedexAPILogic = pokedexAPILogic;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<GenericLookupResult>>> GetPokeballs()
        {
            return await _pokedexAPILogic.GetAllPokeballs();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GenericLookupResult>> GetPokeballById(int id)
        {
            GenericLookupResult pokeball = await _pokedexAPILogic.GetPokeballById(id);

            if (pokeball == null)
            {
                _logger.LogInformation(Constants.InvalidRequest + " for " + Constants.Pokeball + " with Id: " + id);

                return NotFound();
            }

            return pokeball;
        }
    }
}
