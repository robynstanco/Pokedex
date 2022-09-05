using Microsoft.AspNetCore.Mvc;
using Pokedex.Common;
using Pokedex.Logging.Interfaces;
using PokedexAPI.Interfaces;
using PokedexAPI.Models.Output;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PokedexAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AbilitiesController : ControllerBase
    {
        private IPokedexAPILogic _pokedexAPILogic;
        private ILoggerAdapter<AbilitiesController> _logger;
        public AbilitiesController(IPokedexAPILogic pokedexAPILogic, ILoggerAdapter<AbilitiesController> logger)
        {
            _pokedexAPILogic = pokedexAPILogic;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAbilities([FromQuery]int pageNumber = 1, [FromQuery]int pageSize = Constants.PageSize)
        {
            try
            {
                List<LookupResult> paginatedAbilities = await _pokedexAPILogic.GetAllAbilities(pageNumber, pageSize);

                return Ok(paginatedAbilities);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, Constants.UnexpectedError);

                return StatusCode(500);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAbilityById(int id)
        {
            try 
            { 
                LookupResult ability = await _pokedexAPILogic.GetAbilityById(id);

                if (ability == null)
                {
                    _logger.LogInformation($"{Constants.InvalidRequest} for {Constants.Ability}{Constants.WithId}{id}");

                    return NotFound();
                }

                return Ok(ability);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, Constants.UnexpectedError);

                return StatusCode(500);
            }
        }
    }
}