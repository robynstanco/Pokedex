using Microsoft.AspNetCore.Mvc;
using Pokedex.Common;
using Pokedex.Logging.Interfaces;
using PokedexAPI.Interfaces;
using System;
using System.Threading.Tasks;

namespace PokedexAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NationalDexController : ControllerBase
    {
        private readonly IPokedexApiLogic _pokedexApiLogic;
        private readonly ILoggerAdapter<NationalDexController> _logger;
        public NationalDexController(IPokedexApiLogic pokedexApiLogic, ILoggerAdapter<NationalDexController> logger)
        {
            _pokedexApiLogic = pokedexApiLogic ?? throw new ArgumentNullException(nameof(pokedexApiLogic));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<IActionResult> GetNationalDex([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = Constants.PageSize)
        {
            try
            {
                var nationalDex = await _pokedexApiLogic.GetNationalDex(pageNumber, pageSize);

                return Ok(nationalDex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, Constants.UnexpectedError);

                return StatusCode(500);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNationalDexById(int id)
        {
            try
            {
                var nationalDexPokemon = await _pokedexApiLogic.GetNationalDexPokemonById(id);

                if (nationalDexPokemon != null)
                {
                    return Ok(nationalDexPokemon);
                }

                _logger.LogInformation(Constants.InvalidRequest + " for " + Constants.Pokemon + Constants.WithId + id);

                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, Constants.UnexpectedError);

                return StatusCode(500);
            }
        }
    }
}