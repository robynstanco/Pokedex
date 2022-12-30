using Microsoft.AspNetCore.Mvc;
using Pokedex.Common;
using Pokedex.Logging.Interfaces;
using PokedexAPI.Interfaces;
using System;
using System.Threading.Tasks;

namespace PokedexAPI.Controllers
{
    /// <summary>
    /// The abilities controller, readonly operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AbilitiesController : ControllerBase
    {
        private readonly IPokedexApiLogic _pokedexApiLogic;
        private readonly ILoggerAdapter<AbilitiesController> _logger;
        public AbilitiesController(IPokedexApiLogic pokedexApiLogic, ILoggerAdapter<AbilitiesController> logger)
        {
            _pokedexApiLogic = pokedexApiLogic ?? throw new ArgumentNullException(nameof(pokedexApiLogic));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get paginated abilities.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns>The paginated lookup results.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAbilities([FromQuery]int pageNumber = 1, [FromQuery]int pageSize = Constants.PageSize)
        {
            try
            {
                var paginatedAbilities = await _pokedexApiLogic.GetAbilities(pageNumber, pageSize);

                return Ok(paginatedAbilities);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, Constants.UnexpectedError);

                return StatusCode(500);
            }
        }

        /// <summary>
        /// Get the ability by id.
        /// </summary>
        /// <param name="id">The ability id.</param>
        /// <returns>The lookup result.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAbilityById(int id)
        {
            try 
            { 
                var ability = await _pokedexApiLogic.GetAbilityById(id);

                if (ability != null)
                {
                    return Ok(ability);
                }

                _logger.LogInformation($"{Constants.InvalidRequest} for {Constants.Ability}{Constants.WithId}{id}");

                return NotFound();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, Constants.UnexpectedError);

                return StatusCode(500);
            }
        }
    }
}