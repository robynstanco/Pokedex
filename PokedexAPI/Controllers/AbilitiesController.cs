using cloudscribe.Pagination.Models;
using Microsoft.AspNetCore.Mvc;
using Pokedex.Common;
using Pokedex.Common.Interfaces;
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
        private IPaginationHelper _paginationHelper;
        private ILoggerAdapter<AbilitiesController> _logger;
        public AbilitiesController(IPokedexAPILogic pokedexAPILogic, IPaginationHelper paginationHelper,
            ILoggerAdapter<AbilitiesController> logger)
        {
            _pokedexAPILogic = pokedexAPILogic;
            _paginationHelper = paginationHelper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAbilities([FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = Constants.PageSize)
        {
            List<GenericLookupResult> abilities = await _pokedexAPILogic.GetAllAbilities();

            PagedResult<GenericLookupResult> pagedAbilities = 
                _paginationHelper.GetPagedResults(abilities, pageNumber, pageSize);

            return Ok(pagedAbilities.Data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAbilityById(int id)
        {
            GenericLookupResult ability = await _pokedexAPILogic.GetAbilityById(id);

            if (ability == null)
            {
                _logger.LogInformation(Constants.InvalidRequest + " for " + Constants.Ability + Constants.WithId + id);

                return BadRequest();
            }

            return Ok(ability);
        }
    }
}