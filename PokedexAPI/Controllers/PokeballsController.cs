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
        public async Task<IActionResult> GetPokeballs()
        {
            List<GenericLookupResult> pokeballs = await _pokedexAPILogic.GetAllPokeballs();

            return Ok(pokeballs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPokeballById(int id)
        {
            GenericLookupResult pokeball = await _pokedexAPILogic.GetPokeballById(id);

            if (pokeball == null)
            {
                _logger.LogInformation(Constants.InvalidRequest + " for " + Constants.Pokeball + " with Id: " + id);

                return BadRequest();
            }

            return Ok(pokeball);
        }
    }
}
