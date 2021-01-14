using Microsoft.AspNetCore.Mvc;
using Pokedex.Common;
using Pokedex.Logging.Interfaces;
using PokedexAPI.Interfaces;
using PokedexAPI.Models.Output;
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
        public async Task<ActionResult<List<GenericLookupResult>>> GetAbilities()
        {
            return await _pokedexAPILogic.GetAllAbilities();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GenericLookupResult>> GetAbilityById(int id)
        {
            GenericLookupResult ability = await _pokedexAPILogic.GetAbilityById(id);

            if (ability == null)
            {
                _logger.LogInformation(Constants.InvalidRequest + " for " + Constants.Ability + " with Id: " + id);

                return NotFound();
            }

            return ability;
        }
    }
}