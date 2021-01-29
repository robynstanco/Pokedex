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
    public class NationalDexController : ControllerBase
    {
        private IPokedexAPILogic _pokedexAPILogic;
        private IPaginationHelper _paginationHelper;
        private ILoggerAdapter<NationalDexController> _logger;
        public NationalDexController(IPokedexAPILogic pokedexAPILogic, IPaginationHelper paginationHelper,
            ILoggerAdapter<NationalDexController> logger)
        {
            _pokedexAPILogic = pokedexAPILogic;
            _paginationHelper = paginationHelper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetNationalDex([FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = Constants.PageSize)
        {
            List<GenericPokemonResult> nationalDex = await _pokedexAPILogic.GetNationalDex();

            PagedResult<GenericPokemonResult> pagedNationalDex =
                _paginationHelper.GetPagedResults(nationalDex, pageNumber, pageSize);

            return Ok(pagedNationalDex.Data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNationalDexById(int id)
        {
            GenericPokemonResult nationalDexPokemon = await _pokedexAPILogic.GetNationalDexPokemonById(id);

            if (nationalDexPokemon == null)
            {
                _logger.LogInformation(Constants.InvalidRequest + " for " + Constants.Pokemon + Constants.WithId + id);

                return BadRequest();
            }

            return Ok(nationalDexPokemon);
        }
    }
}