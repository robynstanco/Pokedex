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
    public class PokeballsController : ControllerBase
    {
        private IPokedexAPILogic _pokedexAPILogic;
        private IPaginationHelper _paginationHelper;
        private ILoggerAdapter<PokeballsController> _logger;
        public PokeballsController(IPokedexAPILogic pokedexAPILogic, IPaginationHelper paginationHelper,
            ILoggerAdapter<PokeballsController> logger)
        {
            _pokedexAPILogic = pokedexAPILogic;
            _paginationHelper = paginationHelper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetPokeballs([FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = Constants.PageSize)
        {
            List<LookupResult> pokeballs = await _pokedexAPILogic.GetAllPokeballs();

            PagedResult<LookupResult> pagedPokeballs =
                _paginationHelper.GetPagedResults(pokeballs, pageNumber, pageSize);

            return Ok(pagedPokeballs.Data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPokeballById(int id)
        {
            LookupResult pokeball = await _pokedexAPILogic.GetPokeballById(id);

            if (pokeball == null)
            {
                _logger.LogInformation(Constants.InvalidRequest + " for " + Constants.Pokeball + Constants.WithId + id);

                return BadRequest();
            }

            return Ok(pokeball);
        }
    }
}
