using Microsoft.AspNetCore.Mvc;
using Pokedex.Data.Models;
using Pokedex.Logging.Interfaces;
using Pokedex.Repository.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PokedexAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AbilitiesController : ControllerBase
    {
        private IPokedexRepository _pokedexRepository;
        private ILoggerAdapter<AbilitiesController> _logger;
        public AbilitiesController(IPokedexRepository pokedexRepo, ILoggerAdapter<AbilitiesController> logger)
        {
            _pokedexRepository = pokedexRepo;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<tlkpAbility>>> GetAbilities()
        {
            return await _pokedexRepository.GetAllAbilities();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<tlkpAbility>> GetAbilityById(int id)
        {
            tlkpAbility ability = await _pokedexRepository.GetAbilityById(id);

            if (ability == null)
            {
                _logger.LogInformation("No ability with id: " + id);

                return NotFound();
            }

            return ability;
        }
    }
}
